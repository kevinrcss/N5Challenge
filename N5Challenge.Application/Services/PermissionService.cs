using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using N5Challenge.Application.Common;
using N5Challenge.Application.DTOs.Permission;
using N5Challenge.Application.Interfaces;
using N5Challenge.Core.Entities;
using N5Challenge.Core.Interfaces;
using N5Challenge.Infrastructure.Kafka;
using N5Challenge.Infrastructure.Services;
using N5Challenge.Infrastructure.Settings;

namespace N5Challenge.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PermissionService> _logger;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly string _kafkaTopic;

        #region Constructor
        public PermissionService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PermissionService> logger,
            IElasticsearchService elasticsearchService,
            IKafkaProducer kafkaProducer,
            IOptions<KafkaSettings> kafkaSettings)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _elasticsearchService = elasticsearchService;
            _kafkaProducer = kafkaProducer;
            _kafkaTopic = kafkaSettings.Value.Topic;
        }
        #endregion

        public async Task<Result<PermissionDto>> RequestPermissionAsync(PermissionCreateDto permissionDto)
        {
            Result<PermissionDto> result = new();
            try
            {
                var permission = _mapper.Map<Permission>(permissionDto);

                await _unitOfWork.Permissions.AddAsync(permission);
                await _unitOfWork.SaveChangesAsync();

                result.Data = _mapper.Map<PermissionDto>(permission);
                result.Message = Messages.GENERAL_OK;
                result.Success = true;

                _ = Task.WhenAll(
                    IndexPermissionAsync(permission),
                    SendKafkaMessageAsync("request", permission)
                );

            }
            catch (Exception ex)
            {
                result.Message = Messages.GENERAL_EXCEPTION;
                _logger.LogError(ex, string.Concat(ex.Message, ex.InnerException?.Message));
            }
            return result;
        }

        public async Task<Result<PermissionDto>> ModifyPermissionAsync(PermissionUpdateDto permissionDto)
        {
            Result<PermissionDto> result = new();
            try
            {
                Permission permission = await _unitOfWork.Permissions.GetByIdAsync(permissionDto.Id);
                if (permission == null)
                {
                    result.Message = Messages.GENERAL_NOT_FOUND;
                    return result;
                }

                _mapper.Map(permissionDto, permission);

                _unitOfWork.Permissions.Update(permission);
                await _unitOfWork.SaveChangesAsync();

                result.Data = _mapper.Map<PermissionDto>(permission);
                result.Message = Messages.GENERAL_OK;
                result.Success = true;

                _ = Task.WhenAll(
                    UpdateElasticsearchAsync(permission),
                    SendKafkaMessageAsync("modify", permission)
                );
            }
            catch (Exception ex)
            {
                result.Message = Messages.GENERAL_EXCEPTION;
                _logger.LogError(ex, string.Concat(ex.Message, ex.InnerException?.Message));
            }
            return result;
        }

        public async Task<Result<PermissionDto>> GetPermissionByIdAsync(int id)
        {
            Result<PermissionDto> result = new();
            try
            {
                var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
                if (permission == null)
                {
                    result.Message = Messages.GENERAL_NOT_FOUND;
                    return result;
                }
                result.Data = _mapper.Map<PermissionDto>(permission);
                result.Message = Messages.GENERAL_OK;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = Messages.GENERAL_EXCEPTION;
                _logger.LogError(ex, string.Concat(ex.Message, ex.InnerException?.Message));
            }
            return result;
        }

        public async Task<Result<IEnumerable<PermissionDto>>> GetPermissionsAsync(string? searchTerm = null)
        {
            Result<IEnumerable<PermissionDto>> result = new();
            try
            {
                IEnumerable<Permission> permissions;
                if (string.IsNullOrEmpty(searchTerm))
                {
                    permissions = await _unitOfWork.Permissions.GetAllWithTypesAsync();
                }
                else
                {
                    try
                    {
                        permissions = await _elasticsearchService.GetAllPermissionsAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error retrieving permissions from Elasticsearch. Falling back to database.");
                        permissions = await _unitOfWork.Permissions.GetAllAsync();
                    }
                }

                result.Data = _mapper.Map<IEnumerable<PermissionDto>>(permissions);
                result.Message = Messages.GENERAL_OK;
                result.Success = true;

                _ = SendKafkaMessageAsync("get", permissions.Count());

            }
            catch (Exception ex)
            {
                result.Message = Messages.GENERAL_EXCEPTION;
                _logger.LogError(ex, string.Concat(ex.Message, ex.InnerException?.Message));
            }
            return result;
        }

        #region Private Methods
        private async Task IndexPermissionAsync(Permission permission)
        {
            try
            {
                var indexed = await _elasticsearchService.IndexPermissionAsync(permission);
                if (!indexed)
                {
                    _logger.LogWarning("Failed to index permission in Elasticsearch. ID: {PermissionId}", permission.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error indexing permission in Elasticsearch. ID: {PermissionId}", permission.Id);
            }
        }
        private async Task SendKafkaMessageAsync(string operation, object data)
        {
            try
            {
                var kafkaMessage = new { Id = Guid.NewGuid(), Name = operation, Data = data };
                await _kafkaProducer.ProduceAsync(_kafkaTopic, Guid.NewGuid().ToString(), kafkaMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to Kafka. Operation: {Operation}", operation);
            }
        }
        private async Task UpdateElasticsearchAsync(Permission permission)
        {
            try
            {
                await _elasticsearchService.UpdatePermissionAsync(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission in Elasticsearch. ID: {PermissionId}", permission.Id);
            }
        }
        #endregion
    }
}

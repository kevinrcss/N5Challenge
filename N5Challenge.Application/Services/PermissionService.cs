using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using N5Challenge.Application.Common;
using N5Challenge.Application.DTOs;
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

        public async Task<Result<PermissionDto>> RequestPermissionAsync(PermissionDto permissionDto)
        {
            try
            {
                var permission = _mapper.Map<Permission>(permissionDto);

                await _unitOfWork.Permissions.AddAsync(permission);
                await _unitOfWork.SaveChangesAsync();

                // Indexar en Elasticsearch
                var indexed = await _elasticsearchService.IndexPermissionAsync(permission);
                if (!indexed)
                {
                    _logger.LogWarning("Failed to index permission in Elasticsearch. ID: {PermissionId}", permission.Id);
                }

                // Enviar mensaje a Kafka
                var kafkaMessage = new { Id = Guid.NewGuid(), Name = "request", Permission = permission };
                await _kafkaProducer.ProduceAsync(_kafkaTopic, permission.Id.ToString(), kafkaMessage);

                var resultDto = _mapper.Map<PermissionDto>(permission);
                return Result<PermissionDto>.Ok(resultDto, Messages.GENERAL_OK);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result<PermissionDto>.Fail(Messages.GENERAL_ERROR);
            }            
        }

        public async Task<Result<PermissionDto>> ModifyPermissionAsync(PermissionDto permissionDto)
        {
            try
            {
                var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionDto.Id);
                if (permission == null)
                {
                    return Result<PermissionDto>.Fail(Messages.GENERAL_NOT_FOUND);
                }

                _mapper.Map(permissionDto, permission);

                _unitOfWork.Permissions.Update(permission);
                await _unitOfWork.SaveChangesAsync();

                // Update in Elasticsearch
                await _elasticsearchService.UpdatePermissionAsync(permission);

                var kafkaMessage = new { Id = Guid.NewGuid(), Name = "modify", Permission = permission };
                await _kafkaProducer.ProduceAsync(_kafkaTopic, permission.Id.ToString(), kafkaMessage);

                var resultDto = _mapper.Map<PermissionDto>(permission);
                return Result<PermissionDto>.Ok(resultDto, Messages.GENERAL_OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result<PermissionDto>.Fail(Messages.GENERAL_ERROR);
            }
        }
        public async Task<Result<IEnumerable<PermissionDto>>> GetPermissionsAsync(string? searchTerm = null)
        {
            try
            {
                IEnumerable<Permission> permissions;
                if (string.IsNullOrEmpty(searchTerm))
                {
                    permissions = await _unitOfWork.Permissions.GetAllAsync();
                }
                else
                {
                     permissions = await _elasticsearchService.GetAllPermissionsAsync();
                }

                var kafkaMessage = new
                {
                    Id = Guid.NewGuid(),
                    Name = "get",
                    Count = permissions.Count()
                };
                await _kafkaProducer.ProduceAsync(_kafkaTopic, "get_all", kafkaMessage);

                var resultDto = _mapper.Map<IEnumerable<PermissionDto>>(permissions);
                return Result<IEnumerable<PermissionDto>>.Ok(resultDto, Messages.GENERAL_OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result<IEnumerable<PermissionDto>>.Fail(Messages.GENERAL_ERROR);
            }            
        }

        public async Task<Result<PermissionDto>> GetPermissionByIdAsync(int id)
        {
            try
            {
                //var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
                var permission = await _elasticsearchService.GetPermissionAsync(id);
                if (permission == null)
                {
                    return Result<PermissionDto>.Fail(Messages.GENERAL_NOT_FOUND);
                }
                var resultDto = _mapper.Map<PermissionDto>(permission);

                return Result<PermissionDto>.Ok(resultDto, Messages.GENERAL_OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, id);
                return Result<PermissionDto>.Fail(Messages.GENERAL_ERROR);
            }
            
        }

        
    }
}

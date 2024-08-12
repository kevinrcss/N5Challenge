using AutoMapper;
using Microsoft.Extensions.Logging;
using N5Challenge.Application.Common;
using N5Challenge.Application.DTOs;
using N5Challenge.Application.Interfaces;
using N5Challenge.Core.Entities;
using N5Challenge.Core.Interfaces;

namespace N5Challenge.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PermissionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PermissionDto>> RequestPermissionAsync(PermissionDto permissionDto)
        {
            try
            {
                var permission = _mapper.Map<Permission>(permissionDto);
                await _unitOfWork.Permissions.AddAsync(permission);
                await _unitOfWork.SaveChangesAsync();
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
                var resultDto = _mapper.Map<PermissionDto>(permission);
                return Result<PermissionDto>.Ok(resultDto, Messages.GENERAL_OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result<PermissionDto>.Fail(Messages.GENERAL_ERROR);
            }
        }
        public async Task<Result<IEnumerable<PermissionDto>>> GetPermissionsAsync()
        {
            #region WithOutAutoMapper
            //var permissions = await _unitOfWork.Permissions.GetAllAsync();
            //return permissions.Select(p => new PermissionDto
            //{
            //    Id = p.Id,
            //    EmployeeName = p.EmployeeName,
            //    EmployeeLastName = p.EmployeeLastName,
            //    PermissionTypeId = p.PermissionTypeId,
            //    PermissionDate = p.PermissionDate
            //});

            #endregion

            try
            {
                var permissions = await _unitOfWork.Permissions.GetAllAsync();
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
            #region WithOutAutoMapper
            //var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
            //if (permission == null)
            //    return null;

            //return new PermissionDto
            //{
            //    Id = permission.Id,
            //    EmployeeName = permission.EmployeeName,
            //    EmployeeLastName = permission.EmployeeLastName,
            //    PermissionTypeId = permission.PermissionTypeId,
            //    PermissionDate = permission.PermissionDate
            //};

            #endregion
            try
            {
                var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
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

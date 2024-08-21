using AutoMapper;
using Microsoft.Extensions.Logging;
using N5Challenge.Application.Common;
using N5Challenge.Application.DTOs.PermissionType;
using N5Challenge.Application.Interfaces;
using N5Challenge.Core.Interfaces;

namespace N5Challenge.Application.Services
{
    public class PermissionTypeServices : IPermissionTypeServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PermissionTypeServices> _logger;

        #region Constructor
        public PermissionTypeServices(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PermissionTypeServices> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;                
        }
        #endregion

        public async Task<Result<PermissionTypeDto>> GetPermissionTypeByIdAsync(int id)
        {
            Result<PermissionTypeDto> result = new();
            try
            {
                var permissionType = await _unitOfWork.PermissionTypes.GetByIdAsync(id);

                if (permissionType != null) {
                    result.Data = _mapper.Map<PermissionTypeDto>(permissionType);
                    result.Message = Messages.GENERAL_OK;
                    result.Success = true;
                }
                else
                {
                    result.Message = Messages.GENERAL_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                result.Message = Messages.GENERAL_EXCEPTION;
                _logger.LogError(ex, string.Concat(ex.Message, ex.InnerException?.Message));
            }
            return result;
        }

        public async Task<Result<IEnumerable<PermissionTypeDto>>> GetPermissionTypesAsync()
        {
            Result<IEnumerable<PermissionTypeDto>> result = new();
            try
            {
                var permissionTypeCollection = await _unitOfWork.PermissionTypes.GetAllAsync();

                result.Data = _mapper.Map<IEnumerable<PermissionTypeDto>>(permissionTypeCollection);
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
    }
}

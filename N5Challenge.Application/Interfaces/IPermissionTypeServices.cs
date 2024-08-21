using N5Challenge.Application.Common;
using N5Challenge.Application.DTOs.PermissionType;

namespace N5Challenge.Application.Interfaces
{
    public interface IPermissionTypeServices
    {
        Task<Result<PermissionTypeDto>> GetPermissionTypeByIdAsync(int id);
        Task<Result<IEnumerable<PermissionTypeDto>>> GetPermissionTypesAsync();
    }
}

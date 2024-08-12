using N5Challenge.Application.Common;
using N5Challenge.Application.DTOs;

namespace N5Challenge.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<Result<PermissionDto>> RequestPermissionAsync(PermissionDto permissionDto);
        Task<Result<PermissionDto>> ModifyPermissionAsync(PermissionDto permissionDto);
        Task<Result<IEnumerable<PermissionDto>>> GetPermissionsAsync();
        Task<Result<PermissionDto>> GetPermissionByIdAsync(int id);
    }
}

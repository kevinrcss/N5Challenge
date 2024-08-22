using N5Challenge.Application.Common;
using N5Challenge.Application.DTOs.Permission;

namespace N5Challenge.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<Result<PermissionDto>> RequestPermissionAsync(PermissionCreateDto permissionDto);
        Task<Result<PermissionDto>> ModifyPermissionAsync(PermissionUpdateDto permissionDto);
        Task<Result<PermissionDto>> GetPermissionByIdAsync(int id);
        Task<Result<IEnumerable<PermissionDto>>> GetPermissionsAsync(string searchTerm = null);
    }
}

using N5Challenge.Core.Entities;

namespace N5Challenge.Infrastructure.Services
{
    public interface IElasticsearchService
    {
        Task<bool> IndexPermissionAsync(Permission permission);
        Task<Permission> GetPermissionAsync(int id);
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();
        Task<bool> UpdatePermissionAsync(Permission permission);
    }
}

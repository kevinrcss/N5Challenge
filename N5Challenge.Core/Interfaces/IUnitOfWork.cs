using N5Challenge.Core.Entities;

namespace N5Challenge.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Permission> Permissions { get; }
        IRepository<PermissionType> PermissionTypes { get; }
        Task<int> SaveChangesAsync();
    }
}

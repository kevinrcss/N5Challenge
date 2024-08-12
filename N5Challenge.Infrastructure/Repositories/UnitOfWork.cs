using N5Challenge.Core.Entities;
using N5Challenge.Core.Interfaces;
using N5Challenge.Infrastructure.Data;

namespace N5Challenge.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IRepository<Permission> _permissions;
        public IRepository<PermissionType> _permissionTypes;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public IRepository<Permission> Permissions => _permissions ??= new Repository<Permission>(_context);

        public IRepository<PermissionType> PermissionTypes => _permissionTypes ??= new Repository<PermissionType>(_context);


        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

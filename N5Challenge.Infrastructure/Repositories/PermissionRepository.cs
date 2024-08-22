using Microsoft.EntityFrameworkCore;
using N5Challenge.Core.Entities;
using N5Challenge.Core.Interfaces;
using N5Challenge.Infrastructure.Data;

namespace N5Challenge.Infrastructure.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(ApplicationDbContext context) :base(context){}

        public async Task<IEnumerable<Permission>> GetAllWithTypesAsync()
        {
            return await _context.Permissions
                .Include(p => p.PermissionType)
                .ToListAsync();
        }

    }
}

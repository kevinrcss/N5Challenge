using N5Challenge.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N5Challenge.Core.Interfaces
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task<IEnumerable<Permission>> GetAllWithTypesAsync();
    }
}

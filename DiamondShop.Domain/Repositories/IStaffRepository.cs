using DiamondShop.Domain.Models.StaffAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories
{
    public interface IStaffRepository : IBaseRepository<Staff>
    {
        Task<Staff?> GetByIdentityId (string identityId, CancellationToken cancellationToken =default);
    }
}

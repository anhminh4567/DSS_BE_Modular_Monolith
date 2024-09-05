using DiamondShop.Domain.Models.CustomerAggregate;
using DiamondShop.Domain.Models.CustomerAggregate.ValueObjects;
using DiamondShop.Domain.Models.StaffAggregate;
using DiamondShop.Domain.Models.StaffAggregate.ValueObjects;
using DiamondShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class StaffRepository : BaseRepository<Staff>, IStaffRepository
    {
        public StaffRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Staff?> GetByIdentityId(string identityId, CancellationToken cancellationToken = default)
        {
            return await _set.FirstOrDefaultAsync(s => s.IdentityId == identityId,cancellationToken);
        }

        public override async Task Update(Staff entity, CancellationToken token = default)
        {
           
            var userRoles = entity.Roles;
            userRoles.ForEach(role => _dbContext.Entry(role).State = EntityState.Unchanged);
            base.Update(entity, token);
        }
        public override async Task<Staff?> GetById(CancellationToken token = default, params object[] ids)
        {
            StaffId staffId = (StaffId)ids[0];
            var find = await _set.Include(c => c.Roles).FirstOrDefaultAsync(c => c.Id == staffId);
            return find;
        }
    }
    
}

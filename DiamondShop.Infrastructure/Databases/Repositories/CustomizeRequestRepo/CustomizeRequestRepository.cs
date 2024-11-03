using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.CustomizeRequestRepo
{
    internal class CustomizeRequestRepository : BaseRepository<CustomizeRequest>, ICustomizeRequestRepository
    {
        public CustomizeRequestRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }
        public override async Task<CustomizeRequest?> GetById(params object[] ids)
        {
            var id = (CustomizeRequestId)ids[0];
            return await _set.Include(x => x.DiamondRequests).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}

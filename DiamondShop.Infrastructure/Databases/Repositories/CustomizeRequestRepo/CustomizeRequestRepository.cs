using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Repositories;
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
    }
}

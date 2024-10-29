using DiamondShop.Domain.Models.Warranties;
using DiamondShop.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class WarrantyRepository : BaseRepository<Warranty>, IWarrantyRepository
    {
        public WarrantyRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }
    }
}

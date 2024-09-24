using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class DiamondRepository : BaseRepository<Diamond>, IDiamondRepository
    {
        public DiamondRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }
    }
}

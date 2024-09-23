using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class DiamondPriceRepository : BaseRepository<DiamondPrice>, IDiamondPriceRepository
    {
        public DiamondPriceRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }
    }
}

using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo
{
    internal class SideDiamondRepository : BaseRepository<SideDiamondReq>, ISideDiamondRepository
    {
        public SideDiamondRepository(DiamondShopDbContext dbContext) : base(dbContext) { }
        public async Task CreateOpts(List<SideDiamondOpt> sideDiamondOpts, CancellationToken token = default)
        {
            await _dbContext.Set<SideDiamondOpt>().AddRangeAsync(sideDiamondOpts, token);
        }

    }
}

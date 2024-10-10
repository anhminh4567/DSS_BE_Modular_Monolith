using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo
{
    internal class SideDiamondRepository : BaseRepository<SideDiamondReq>, ISideDiamondRepository
    {
        DbSet<SideDiamondOpt> setOpt;
        public SideDiamondRepository(DiamondShopDbContext dbContext) : base(dbContext) { setOpt = _dbContext.Set<SideDiamondOpt>(); }
        public async Task CreateRange(List<SideDiamondOpt> sideDiamondOpts, CancellationToken token = default)
        {
            await setOpt.AddRangeAsync(sideDiamondOpts, token);
        }

        public async Task<List<SideDiamondOpt>?> GetSideDiamondOption(List<SideDiamondOptId> sideDiamondOptId)
        {
            
            return setOpt.Include(p => p.SideDiamondReq).Where(p => sideDiamondOptId.Contains(p.Id)).ToList();
        }
    }
}

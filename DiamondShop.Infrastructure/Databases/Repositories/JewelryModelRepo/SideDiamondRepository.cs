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
    internal class SideDiamondRepository : BaseRepository<SideDiamondOpt>, ISideDiamondRepository
    {
        public SideDiamondRepository(DiamondShopDbContext dbContext) : base(dbContext) { }

        public async Task CreateRange(List<SideDiamondOpt> sideDiamondOpts, CancellationToken token = default)
        {
            await _set.AddRangeAsync(sideDiamondOpts, token);
        }

        public override async Task<SideDiamondOpt?> GetById(params object[] ids)
        {
            var id = (SideDiamondOptId)ids[0];
            return await _set.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<List<SideDiamondOpt>?> GetSideDiamondOption(List<SideDiamondOptId> sideDiamondOptId)
        {
            return _set.Where(p => sideDiamondOptId.Contains(p.Id)).ToList();
        }
    }
}

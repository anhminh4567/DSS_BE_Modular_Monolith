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
    internal class MainDiamondRepository : BaseRepository<MainDiamondReq>, IMainDiamondRepository
    {
        public MainDiamondRepository(DiamondShopDbContext dbContext) : base(dbContext) { }

        public async Task<List<MainDiamondReq>> GetCriteria(JewelryModelId modelId)
        {
            var mainDiamondReq = _set.Where(p => p.ModelId == modelId).Include(p => p.Shapes);
            return await mainDiamondReq.ToListAsync();
        }

        public async Task CreateRange(List<MainDiamondShape> shapes, CancellationToken token = default)
        {
            await _dbContext.Set<MainDiamondShape>().AddRangeAsync(shapes,token);
        }
    }
}

using DiamondShop.Domain.Models.JewelryModels.Entities;
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

        public List<MainDiamondReq> GetMainDiamondShapes(string modelId)
        {
            var mainDiamondReq = _set.Where(p => p.ModelId.Value == modelId).Include(p => p.Shapes);
            return mainDiamondReq.ToList();
        }

        public async Task CreateShapes(List<MainDiamondShape> shapes, CancellationToken token = default)
        {
            await _dbContext.Set<MainDiamondShape>().AddRangeAsync(shapes,token);
        }
    }
}

using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
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
        public async Task CreateShapes(List<MainDiamondShape> shapes, CancellationToken token = default)
        {
            await _dbContext.Set<MainDiamondShape>().AddRangeAsync(shapes,token);
        }
    }
}

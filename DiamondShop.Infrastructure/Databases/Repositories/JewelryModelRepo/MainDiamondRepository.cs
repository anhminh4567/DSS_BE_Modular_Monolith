using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo
{
    internal class MainDiamondRepository : BaseRepository<MainDiamondReq>, IMainDiamondRepository
    {
        private readonly IMemoryCache _memoryCache;
        public MainDiamondRepository(DiamondShopDbContext dbContext, IMemoryCache memoryCache) : base(dbContext)
        {
            _memoryCache = memoryCache;
        }

        public async Task<List<MainDiamondReq>> GetCriteria(JewelryModelId modelId)
        {
            //string key = $"MD_{modelId}";
            //var value = (List<MainDiamondReq>)_memoryCache.Get(key);
            //if (value == null)
            //{
            //    value = await _set.Where(p => p.ModelId == modelId).Include(p => p.Shapes).ToListAsync();
            //    _memoryCache.Set(key, value, TimeSpan.FromHours(4));
            //}
            var value = await _set.Where(p => p.ModelId == modelId).Include(p => p.Shapes).AsNoTracking().ToListAsync();
            return value;
        }

        public async Task CreateRange(List<MainDiamondShape> shapes, CancellationToken token = default)
        {
            await _dbContext.Set<MainDiamondShape>().AddRangeAsync(shapes, token);
        }
    }
}

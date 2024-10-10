using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo
{
    internal class SizeMetalRepository : BaseRepository<SizeMetal>, ISizeMetalRepository
    {
        private readonly IMemoryCache _cache;
        public SizeMetalRepository(DiamondShopDbContext dbContext, IMemoryCache cache) : base(dbContext)
        {
            _cache = cache;
        }

        public Task CreateRange(List<SizeMetal> sizeMetalList, CancellationToken token) => _set.AddRangeAsync(sizeMetalList, token);
    }
}

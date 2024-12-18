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
        public override async Task<SizeMetal?> GetById(params object[] ids)
        {
            return await _set.Include(p => p.Size).FirstOrDefaultAsync(p => p.ModelId == (JewelryModelId)ids[0] && p.MetalId == (MetalId)ids[1] && p.SizeId == (SizeId)ids[2]);
        }
        public async Task CreateRange(List<SizeMetal> sizeMetalList, CancellationToken token) => await _set.AddRangeAsync(sizeMetalList, token);

        public void UpdateRange(List<SizeMetal> sizeMetalList, CancellationToken token)
        {
            _set.UpdateRange(sizeMetalList);
        }
        public async Task<bool> Existing(JewelryModelId modelId, MetalId metalId, SizeId sizeId)
        {
            return await _set.Where(p => p.ModelId == modelId && p.MetalId == metalId && p.SizeId == sizeId).AnyAsync();
        }
    }
}

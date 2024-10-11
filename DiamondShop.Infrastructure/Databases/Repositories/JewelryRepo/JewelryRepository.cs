using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryRepo
{
    internal class JewelryRepository : BaseRepository<Jewelry>, IJewelryRepository
    {
        private readonly IMemoryCache _cache;
        public JewelryRepository(DiamondShopDbContext dbContext, IMemoryCache cache) : base(dbContext)
        {
            _cache = cache;
        }
        public override async Task<Jewelry?> GetById(params object[] ids)
        {
            JewelryId id = (JewelryId)ids[0];
            return await _set.Include(d => d.SideDiamonds).FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task<bool> CheckDuplicatedSerial(string serialNumber)
        {
            return await _set.AnyAsync(p => p.SerialCode == serialNumber);
        }

        public async Task<(IEnumerable<Jewelry> jewelries, int totalPage)> GetSellingJewelry(int skip, int take)
        {
            var list = _set.Where(p => p.IsActive).ToList();
            var sizeMetalSet = _dbContext.Set<SizeMetal>();
            var count = list.Count();
            var result = list.Skip(skip).Take(take);
            foreach (var i in result)
            {
                string modelKey = $"MS_{i.ModelId.Value}";
                //List<SizeMetal> tryGet = _cache.Get<List<SizeMetal>>(modelKey) ?? new List<SizeMetal>();

                List<SizeMetal> tryGet = new();
                var item = tryGet.FirstOrDefault(p => p.MetalId == i.MetalId && p.SizeId == i.SizeId);
                if (item == null)
                {
                    item = await sizeMetalSet.Include(p => p.Metal).FirstOrDefaultAsync(
                        p =>
                        p.ModelId == i.ModelId &&
                        p.MetalId == i.MetalId &&
                        i.SizeId == p.SizeId
                    );
                    tryGet.Add(item);
                    //_cache.Set(modelKey, tryGet);
                }
                i.Price = item.Metal.Price * (decimal)item.Weight;
            };
            var totalPage = (int)Math.Ceiling((decimal)count / (decimal)take);
            return (result, totalPage);
        }
    }
}

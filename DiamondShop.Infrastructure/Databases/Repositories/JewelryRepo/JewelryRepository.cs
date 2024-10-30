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
            return await _set.Include(d => d.SideDiamond)
                .Include(d => d.Diamonds)
                .ThenInclude(d => d.DiamondShape).FirstOrDefaultAsync(d => d.Id == id);
        }
        public void UpdateRange(List<Jewelry> jewelries)
        {
            _set.UpdateRange(jewelries);
        }

        public async Task<bool> CheckDuplicatedSerial(string serialNumber)
        {
            return await _set.AnyAsync(p => p.SerialCode == serialNumber);
        }

        public Task<bool> IsHavingDiamond(Jewelry jewelry, CancellationToken cancellationToken = default)
        {
            return _dbContext.Diamonds.AnyAsync(d => d.JewelryId == jewelry.Id, cancellationToken);
        }
    }
}

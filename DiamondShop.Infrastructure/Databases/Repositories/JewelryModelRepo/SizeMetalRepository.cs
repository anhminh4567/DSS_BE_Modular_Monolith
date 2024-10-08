using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo
{
    internal class SizeMetalRepository : BaseRepository<SizeMetal>, ISizeMetalRepository
    {
        public SizeMetalRepository(DiamondShopDbContext dbContext) : base(dbContext) { }
        public Task CreateRange(List<SizeMetal> sizeMetalList, CancellationToken token) => _set.AddRangeAsync(sizeMetalList,token);
    }
}

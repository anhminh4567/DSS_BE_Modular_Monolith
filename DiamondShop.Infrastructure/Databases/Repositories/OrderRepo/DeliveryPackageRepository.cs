using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using Microsoft.EntityFrameworkCore;

namespace DiamondShop.Infrastructure.Databases.Repositories.OrderRepo
{
    internal class DeliveryPackageRepository : BaseRepository<DeliveryPackage>, IDeliveryPackageRepository
    {
        public DeliveryPackageRepository(DiamondShopDbContext dbContext) : base(dbContext) { }
        public override async Task<DeliveryPackage?> GetById(params object[] ids)
        {
            DeliveryPackageId id = (DeliveryPackageId)ids[0];
            return await _set.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}

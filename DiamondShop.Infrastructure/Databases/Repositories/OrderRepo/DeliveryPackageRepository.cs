using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Repositories.OrderRepo;

namespace DiamondShop.Infrastructure.Databases.Repositories.OrderRepo
{
    internal class DeliveryPackageRepository : BaseRepository<DeliveryPackage>, IDeliveryPackageRepository
    {
        public DeliveryPackageRepository(DiamondShopDbContext dbContext) : base(dbContext) { }
    }
}

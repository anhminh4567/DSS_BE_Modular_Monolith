using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using Microsoft.EntityFrameworkCore;

namespace DiamondShop.Infrastructure.Databases.Repositories.OrderRepo
{
    internal class OrderItemWarrantyRepository : BaseRepository<OrderItemWarranty> , IOrderItemWarrantyRepository
    {
        public OrderItemWarrantyRepository(DiamondShopDbContext dbContext) : base(dbContext) { }
        public override async Task<OrderItemWarranty?> GetById(params object[] ids)
        {
            var id = (OrderItemId)ids[0];
            return await _set.FindAsync(id);
        }
        public async Task CreateRange(List<OrderItemWarranty> warranties)
        {
            await _set.AddRangeAsync(warranties);
        }
    }
}

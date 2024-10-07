using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Repositories.OrderRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.OrderRepo
{
    internal class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {

        }
    }
}

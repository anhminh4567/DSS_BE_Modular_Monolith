using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Repositories.OrderRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.OrderRepo
{
    internal class OrderLogRepository : BaseRepository<OrderLog>, IOrderLogRepository
    {
        public OrderLogRepository(DiamondShopDbContext dbContext) : base(dbContext) { }
    }
}

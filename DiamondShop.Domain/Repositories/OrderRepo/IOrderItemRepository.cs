using DiamondShop.Domain.Models.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.OrderRepo
{
    public interface IOrderItemRepository : IBaseRepository<OrderItem>
    {
        public Task CreateRange(List<OrderItem> orderItems);
        public void UpdateRange(List<OrderItem> orderItems);
    }
}

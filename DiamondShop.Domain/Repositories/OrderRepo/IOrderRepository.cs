using DiamondShop.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.OrderRepo
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
    }
}

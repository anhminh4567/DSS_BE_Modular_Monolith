using DiamondShop.Domain.Models.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.OrderRepo
{
    public interface IOrderItemWarrantyRepository : IBaseRepository<OrderItemWarranty>
    {
        public Task CreateRange(List<OrderItemWarranty> warranties);
    }
}

using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Entities
{
    public class OrderLog : Entity<OrderLogId>
    {
        public OrderId OrderId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public OrderLogId PreviousLogId { get; set; }
        public OrderLog PreviousLog { get; set; }
        public OrderLog() { }
    }
}

using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Deliveries.Enum;
using DiamondShop.Domain.Models.Deliveries.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Deliveries
{
    public class Delivery : Entity<DeliveryId>
    {
        public DateTime DeliveringDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public DeliveryStatus Status { get; set; }
        public Account Account { get; set; }
        public AccountId AccountId { get; set; }
        public List<Order> Orders { get; set; } = new ();
        public Delivery() { }
    }
}

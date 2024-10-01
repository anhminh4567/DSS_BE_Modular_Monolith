using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Entities
{
    public class DeliveryPackage : Entity<DeliveryPackageId>
    {
        public DateTime DeliveryDate { get; set; }
        public DateTime CompleteDate { get; set; }
        public DeliveryPackageStatus Status { get; set; }
        public string? DeliveryMethod { get; set; }
        public AccountId DelivererId { get; set; }
        public Account? Deliverer { get; set; }
        private DeliveryPackage() { }
    }
}

using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DeliveryFees.Enum;
using DiamondShop.Domain.Models.DeliveryFees.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DeliveryFees
{
    public class DeliveryFee : Entity<DeliveryFeeId>
    {
        public DeliveryMethod Method { get; set; }
        public decimal Fee { get; set; }
        public int FromRange { get; set; }
        public int ToRange { get; set; }
        public DeliveryFee() { }
    }
}

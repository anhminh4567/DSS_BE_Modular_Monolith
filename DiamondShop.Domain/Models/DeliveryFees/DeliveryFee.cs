using DiamondShop.Domain.Common;
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
        public string DeliveryMethod { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public int FromKm { get; set; }
        public int ToKm { get; set; }

        private DeliveryFee()
        {
        }
    }
}

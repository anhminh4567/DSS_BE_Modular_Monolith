using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Warranties.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Jewelries.Entities
{
    public class JewelryWarranty : Entity<JewelryId>
    {
        public WarrantyStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public WarrantyType Type { get; set; }
        public string WarrantyPath { get; set; }
    }
}

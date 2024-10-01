using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public class Size : Entity<SizeId>
    {
        public string Unit { get; set; }
        public float Value { get; set; }
        public Size() { }
        public static Size Create(float value, string unit = null, SizeId givenId = null) => new Size() { Id = givenId is null ? SizeId.Create() : givenId, Unit = unit is null ? SizeRules.DefaultUnit : unit, Value = value };
    }
}

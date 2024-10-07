using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

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

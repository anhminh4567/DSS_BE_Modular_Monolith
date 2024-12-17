using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public class Size : Entity<SizeId>
    {
        public static string Milimeter { get; set; } = "milimeter";
        public static string Centimeter { get; set; } = "centimeter";
        public string Unit { get; set; }
        public float Value { get; set; }

        public Size() { }
        public static Size Create(float value, string unit, SizeId givenId = null) => new Size() { Id = givenId is null ? SizeId.Create() : givenId, Unit = unit, Value = value };
    }
}

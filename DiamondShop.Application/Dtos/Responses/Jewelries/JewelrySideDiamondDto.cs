using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.JewelryModels.Enum;

namespace DiamondShop.Application.Dtos.Responses.Jewelries
{
    public class JewelrySideDiamondDto
    {
        public string Id { get; set; }
        public string JewelryId { get; set; }
        public float Carat { get; set; }
        public int Quantity { get; set; }
        public Color ColorMin { get; set; }
        public Color ColorMax { get; set; }
        public Clarity ClarityMin { get; set; }
        public Clarity ClarityMax { get; set; }
        public SettingType SettingType { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}

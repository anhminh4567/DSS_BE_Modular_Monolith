using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Application.Dtos.Responses.JewelryModels
{
    public class JewelryModelSellingDto
    {
        public string Name { get; set; }
        public MediaDto? Thumbnail { get; set; }
        public int StarRating { get; set; }
        public int ReviewCount { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string JewelryModelId { get; set; }
        public string MetalId { get; set; }
        public string SideDiamondOptId { get; set; }
    }
}

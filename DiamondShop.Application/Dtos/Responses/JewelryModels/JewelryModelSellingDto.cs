using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Application.Dtos.Responses.JewelryModels
{
    public class JewelryModelSellingDto
    {
        public string Name { get; set; }
        public string ThumbnailPath { get; set; }
        public int StarRating { get; set; }
        public int ReviewCount { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public JewelryModelId JewelryModelId { get; set; }
        public MetalId MetalId { get; set; }
        public SideDiamondOptId SideDiamondOptId { get; set; }
    }
}

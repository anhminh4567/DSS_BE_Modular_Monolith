using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Common.ValueObjects;

namespace DiamondShop.Application.Dtos.Responses.Jewelries
{
    public class JewelryDto
    {
        public string Id { get; set; }
        public string SerialCode { get; set; }
        public string ModelId { get; set; }
        public string ModelCode { get; set; }
        public JewelryModelCategoryDto? Category { get; set; }
        public string SizeId { get; set; }
        public SizeDto Size { get; set; }
        public string MetalId { get; set; }
        public MetalDto Metal { get; set; }
        public float Weight { get; set; }
        public ProductStatus Status { get; set; }
        public DateTime ShippingDate { get; set; }
        public List<DiamondDto> Diamonds { get; set; } = new();
        public JewelrySideDiamondDto? SideDiamond { get; set; } = new();
        public string ReviewId { get; set; }
        public JewelryReviewDto? Review { get; set; }
        public Media Thumbnail { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsPreset { get; set; }
        public decimal? ND_Price { get; set; }
        public decimal? D_Price { get; set; }
        public decimal? SD_Price { get; set; }
        public decimal? SoldPrice { get; set; }
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
    }
}

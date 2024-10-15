using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Domain.Common.ValueObjects;

namespace DiamondShop.Application.Dtos.Responses.Jewelries
{
    public class JewelryDto
    {
        public string Id { get; set; }
        public string ModelId { get; set; }
        public JewelryModelDto Model { get; set; }
        public string SizeId { get; set; }
        public SizeDto Size { get; set; }
        public string MetalId { get; set; }
        public MetalDto Metal { get; set; }
        public float Weight { get; set; }
        public string SerialCode { get; set; }
        public bool IsAwaiting { get; set; }
        public bool IsSold { get; set; }
        public DateTime ShippingDate { get; set; }
        public List<DiamondDto> Diamonds { get; set; } = new();
        public List<JewelrySideDiamondDto>? SideDiamonds { get; set; } = new();
        public string ReviewId { get; set; }
        public JewelryReviewDto? Review { get; set; }
        public Media Thumbnail { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsPreset { get; set; }

    }
}

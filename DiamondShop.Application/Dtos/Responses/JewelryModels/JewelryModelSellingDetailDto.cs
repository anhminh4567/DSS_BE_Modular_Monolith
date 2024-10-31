using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.Enum;

namespace DiamondShop.Application.Dtos.Responses.JewelryModels
{
    public class JewelryModelSellingDetailDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public float? Width { get; set; }
        public float? Length { get; set; }
        public bool IsEngravable { get; set; }
        public bool IsRhodiumFinish { get; set; }
        public BackType? BackType { get; set; }
        public ClaspType? ClaspType { get; set; }
        public ChainType? ChainType { get; set; }
        public List<SellingDetailMetalDto> MetalGroups;
        public List<SideDiamondOptDto>? SideDiamonds { get; set; } = new();
        public List<JewelryReviewDto>? Reviews { get; set; }
    }
    public class SellingDetailMetalDto
    {
        public string Name { get; set; }
        public MetalDto Metal { get; set; }
        public string? SideDiamondId { get; set; }
        public List<string>? Images { get; set; } = new();
        List<SellingDetailSizeDto> SizeGroups { get; set; } = new();
    }
    public class SellingDetailSizeDto
    {
        public float Size { get; set; }
        public decimal Price { get; set; }
    }
}
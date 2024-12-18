using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.Enum;

namespace DiamondShop.Application.Dtos.Responses.JewelryModels
{
    public class JewelryModelSellingDetailDto
    {
        public string Id { get; set; }
        public string Unit { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public float? Width { get; set; }
        public bool IsEngravable { get; set; }
        public bool IsRhodiumFinish { get; set; }
        public BackType? BackType { get; set; }
        public ClaspType? ClaspType { get; set; }
        public ChainType? ChainType { get; set; }
        public bool HasMainDiamond { get; set; }
        public List<MainDiamondReqDto> MainDiamonds { get; set; } = new();
        public List<SellingDetailMetalDto> MetalGroups;
        public List<MetalDto>? Metals { get; set; } = new();
        public List<SideDiamondOptDto>? SideDiamonds { get; set; } = new();
        public List<JewelryReviewDto>? Reviews { get; set; }
        public MediaDto? Thumbnail { get; set; }
        public JewelryModelGalleryTemplateDto? GalleryTemplate { get; set; }
    }
    public class SellingDetailMetalDto
    {
        public string Name { get; set; }
        public string MetalId { get; set; }
        public string? SideDiamondId { get; set; }
        public bool IsPriced { get; set; }
        public List<MediaDto>? Images { get; set; } = new();
        public List<SellingDetailSizeDto> SizeGroups { get; set; } = new();
    }
    public class SellingDetailSizeDto
    {
        public float Size { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsInStock { get; set; }
    }
}
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetSellingDetail
{
    public class JewelryModelSellingDetail
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
        public List<SellingDetailMetal> MetalGroups { get; set; } = new();
        public List<SideDiamondOpt>? SideDiamonds { get; set; } = new();
        public List<JewelryReview>? Reviews { get; set; } = new();
        public static JewelryModelSellingDetail Create(JewelryModel model, List<SellingDetailMetal> MetalGroups,
            List<SideDiamondOpt>? sideDiamondOpts, List<JewelryReview>? reviews)
        {
            return new JewelryModelSellingDetail()
            {
                Name = model.Name,
                Category = model.Category.Name,
                Width = model.Width,
                Length = model.Length,
                IsEngravable = model.IsEngravable,
                IsRhodiumFinish = model.IsRhodiumFinish,
                BackType = model.BackType,
                ClaspType = model.ClaspType,
                ChainType = model.ChainType,
                MetalGroups = MetalGroups,
                SideDiamonds = sideDiamondOpts,
                Reviews = reviews
            };
        }
    }
    public class SellingDetailMetal
    {
        public string Name { get; set; }
        public Metal Metal { get; set; }
        public SideDiamondOptId? SideDiamondId { get; set; }
        public List<string>? Images { get; set; } = new();
        public List<SellingDetailSize> SizeGroups { get; set; } = new();
        public static SellingDetailMetal CreateWithSide(string modelName, Metal metal, SideDiamondOpt sideDiamondOpt,
            List<string> images, List<SellingDetailSize> sizeGroup)
        {
            return new SellingDetailMetal
            {
                Name = $"{modelName} in {metal.Name} ({sideDiamondOpt.CaratWeight} Tw)",
                Metal = metal,
                SideDiamondId = sideDiamondOpt.Id,
                Images = images,
                SizeGroups = sizeGroup
            };
        }
        public static SellingDetailMetal CreateNoSide(string modelName, Metal metal,
            List<string> images, List<SellingDetailSize> sizeGroup)
        {
            return new SellingDetailMetal
            {
                Name = $"{modelName} in {metal.Name}",
                Metal = metal,
                Images = images,
                SizeGroups = sizeGroup
            };
        }
    }
    public class SellingDetailSize
    {
        public float Size { get; set; }
        public decimal Price { get; set; }
        public bool IsInStock { get; set; }
        public static SellingDetailSize Create(float sizeValue, decimal Price, bool isInStock)
        {
            return new SellingDetailSize { 
                Size = sizeValue, 
                Price = Price,
                IsInStock = isInStock
            };
        }
    }
}
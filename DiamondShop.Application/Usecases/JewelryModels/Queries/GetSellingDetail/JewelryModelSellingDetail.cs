using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Services.Implementations;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetSellingDetail
{
    public class JewelryModelSellingDetail
    {
        public JewelryModelId Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public float? Width { get; set; }
        public float? Length { get; set; }
        public bool IsEngravable { get; set; }
        //public bool IsRhodiumFinish { get; set; }
        public BackType? BackType { get; set; }
        public ClaspType? ClaspType { get; set; }
        public ChainType? ChainType { get; set; }
        public bool HasMainDiamond { get; set; }
        public List<SellingDetailMetal> MetalGroups { get; set; } = new();
        public List<MainDiamondReq> MainDiamonds { get; set; } = new();
        public List<Metal> Metals { get; set; } = new();
        public List<SideDiamondOpt>? SideDiamonds { get; set; } = new();
        public List<JewelryReview>? Reviews { get; set; } = new();
        public static JewelryModelSellingDetail Create(JewelryModel model, List<SellingDetailMetal> MetalGroups,
            List<SideDiamondOpt>? sideDiamondOpts, List<Metal> metals, List<JewelryReview>? reviews)
        {
            return new JewelryModelSellingDetail()
            {
                Id = model.Id,
                Name = model.Name,
                Category = model.Category.Name,
                Width = model.Width,
                Length = model.Length,
                IsEngravable = model.IsEngravable,
                //IsRhodiumFinish = model.IsRhodiumFinish,
                BackType = model.BackType,
                ClaspType = model.ClaspType,
                ChainType = model.ChainType,
                MainDiamonds = model.MainDiamonds,
                MetalGroups = MetalGroups,
                SideDiamonds = sideDiamondOpts,
                Metals = metals,
                Reviews = reviews,
                HasMainDiamond = model.MainDiamonds.Count > 0,
            };
        }
        public void AssignDiscount(List<Discount> activeDiscounts)
        {
            foreach (var metal in MetalGroups)
            {
                var fakeJewelryFromSellingModel = Jewelry.Create(Id, SizeId.Parse("NONE"), metal.MetalId, 0, "NONE", Domain.Common.Enums.ProductStatus.Active, JewelryId.Parse("NONE"));
                var sizeGroup = metal.SizeGroups;
                foreach (var size in sizeGroup)
                {
                    fakeJewelryFromSellingModel.ND_Price = size.Price ;
                    fakeJewelryFromSellingModel.D_Price = 0;
                    JewelryService.AssignJewelryDiscountGlobal(fakeJewelryFromSellingModel, activeDiscounts).Wait();
                    var salePrice = fakeJewelryFromSellingModel.SalePrice;
                    size.SetSalePrice(salePrice);
                }
            }
        }
    }
    public class SellingDetailMetal
    {
        public string Name { get; set; }
        public MetalId MetalId { get; set; }
        public SideDiamondOptId? SideDiamondId { get; set; }
        public bool IsPriced { get; set; }
        public List<Media>? Images { get; set; } = new();
        public List<SellingDetailSize> SizeGroups { get; set; } = new();
        public static SellingDetailMetal CreateWithSide(string modelName, Metal metal, bool isPriced, SideDiamondOpt sideDiamondOpt,
            List<Media>? images, List<SellingDetailSize> sizeGroup)
        {
            return new SellingDetailMetal
            {
                Name = $"{modelName} in {metal.Name} ({sideDiamondOpt.CaratWeight} Tw)",
                MetalId = metal.Id,
                SideDiamondId = sideDiamondOpt.Id,
                IsPriced = isPriced,
                Images = images,
                SizeGroups = sizeGroup
            };
        }
        public static SellingDetailMetal CreateNoSide(string modelName, Metal metal,
            List<Media>? images, List<SellingDetailSize> sizeGroup)
        {
            return new SellingDetailMetal
            {
                Name = $"{modelName} in {metal.Name}",
                MetalId = metal.Id,
                Images = images,
                SizeGroups = sizeGroup
            };
        }
    }
    public class SellingDetailSize
    {
        public float Size { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; } = 0;
        public bool IsInStock { get; set; }
        public static SellingDetailSize Create(float sizeValue, decimal Price, bool isInStock)
        {
            return new SellingDetailSize
            {
                Size = sizeValue,
                Price = Price,
                IsInStock = isInStock
            };
        }
        public void SetSalePrice (decimal salePrice)
        {
            SalePrice = salePrice;
        }
    }
}
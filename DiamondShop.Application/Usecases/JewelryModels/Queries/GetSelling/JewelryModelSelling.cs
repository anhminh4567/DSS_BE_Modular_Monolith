using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Services.Implementations;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetSelling
{
    public class JewelryModelSelling
    {
        public string Name { get; set; }
        public Media? Thumbnail { get; set; }
        public int StarRating { get; set; }
        public int ReviewCount { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPriceAfterDiscount { get; set; }
        public decimal MaxPriceAfterDiscount { get; set; }
        public JewelryModelId JewelryModelId { get; set; }
        public MetalId MetalId { get; set; }
        public SideDiamondOptId SideDiamondOptId { get; set; }
        public static JewelryModelSelling CreateWithSide(Media? thumbnail, string modelName, string metalName, int star, int reviewCount,
            decimal craftmanFee, decimal minMetalPrice, decimal maxMetalPrice,
            JewelryModelId modelId, MetalId metalId, SideDiamondOpt sideDiamondOpt)
        {
            return new JewelryModelSelling()
            {
                Name = $"{modelName} in {metalName} ({sideDiamondOpt.CaratWeight} Tw)",
                Thumbnail = thumbnail,
                StarRating = 0,
                ReviewCount = 0,
                MinPrice = craftmanFee + minMetalPrice + sideDiamondOpt.TotalPrice,
                MaxPrice = craftmanFee + maxMetalPrice + sideDiamondOpt.TotalPrice,
                JewelryModelId = modelId,
                MetalId = metalId,
                SideDiamondOptId = sideDiamondOpt.Id
            };
        }
        public static JewelryModelSelling CreateNoSide(Media? thumbnail, string modelName, string metalName, int star, int reviewCount,
            decimal craftmanFee, decimal minMetalPrice, decimal maxMetalPrice,
            JewelryModelId modelId, MetalId metalId)
        {
            return new JewelryModelSelling()
            {
                Name = $"{modelName} in {metalName}",
                Thumbnail = thumbnail,
                StarRating = 0,
                ReviewCount = 0,
                MinPrice = craftmanFee + minMetalPrice,
                MaxPrice = craftmanFee + maxMetalPrice,
                JewelryModelId = modelId,
                MetalId = metalId
            };
        }
        public void AssignDiscount(List<Discount> activeDiscounts)
        {
            //for min price;
            var fakeJewelryFromSellingModel = Jewelry.Create(JewelryModelId,SizeId.Parse("NONE"),MetalId,0,"NONE",Domain.Common.Enums.ProductStatus.Active,JewelryId.Parse("NONE"));
            fakeJewelryFromSellingModel.ND_Price = MinPrice;
            fakeJewelryFromSellingModel.D_Price = 0;
            JewelryService.AssignJewelryDiscountGlobal(fakeJewelryFromSellingModel, activeDiscounts).Wait();
            MinPriceAfterDiscount = fakeJewelryFromSellingModel.SalePrice;
            //for max price;
            var fakeJewelryFromSellingModelMaxPrice = Jewelry.Create(JewelryModelId, SizeId.Parse("NONE"), MetalId, 0, "NONE", Domain.Common.Enums.ProductStatus.Active, JewelryId.Parse("NONE"));
            fakeJewelryFromSellingModelMaxPrice.ND_Price = MaxPrice;
            fakeJewelryFromSellingModelMaxPrice.D_Price = 0;
            JewelryService.AssignJewelryDiscountGlobal(fakeJewelryFromSellingModelMaxPrice, activeDiscounts).Wait();
            MaxPriceAfterDiscount = fakeJewelryFromSellingModelMaxPrice.SalePrice;
        }
    }
}
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetSelling
{
    public class JewelryModelSelling
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
        public static JewelryModelSelling CreateWithSide(string thumbnail, string modelName, string metalName, int star, int reviewCount,
            decimal craftmanFee, decimal minMetalPrice, decimal maxMetalPrice, float caratWeight,
            JewelryModelId modelId, MetalId metalId, SideDiamondOptId sideDiamondOptId)
        {
            return new JewelryModelSelling()
            {
                Name = $"{modelName} in {metalName} ({caratWeight} Tw)",
                ThumbnailPath = "",
                StarRating = 0,
                ReviewCount = 0,
                MinPrice = craftmanFee + minMetalPrice,
                MaxPrice = craftmanFee + maxMetalPrice,
                JewelryModelId = modelId,
                MetalId = metalId,
                SideDiamondOptId = sideDiamondOptId
            };
        }
        public static JewelryModelSelling CreateNoSide(string thumbnail, string modelName, string metalName, int star, int reviewCount,
            decimal craftmanFee, decimal minMetalPrice, decimal maxMetalPrice,
            JewelryModelId modelId, MetalId metalId)
        {
            return new JewelryModelSelling()
            {
                Name = $"{modelName} in {metalName}",
                ThumbnailPath = "",
                StarRating = 0,
                ReviewCount = 0,
                MinPrice = craftmanFee + minMetalPrice,
                MaxPrice = craftmanFee + maxMetalPrice,
                JewelryModelId = modelId,
                MetalId = metalId
            };
        }
    }
}
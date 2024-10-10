using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Domain.Models.JewelryModels
{

    public class JewelryModel : Entity<JewelryModelId>, IAggregateRoot
    {
        public string Name { get; set; }
        public JewelryModelCategoryId CategoryId { get; set; }
        public JewelryModelCategory Category { get; set; }
        public float? Width { get; set; }
        public float? Length { get; set; }
        public bool IsEngravable { get; set; }
        public bool IsRhodiumFinish { get; set; }
        public BackType? BackType { get; set; }
        public ClaspType? ClaspType { get; set; }
        public ChainType? ChainType { get; set; }
        public List<MainDiamondReq> MainDiamonds { get; set; } = new ();
        public List<SideDiamondReq> SideDiamonds { get; set; } = new();
        public List<SizeMetal> SizeMetals { get; set; } = new();
        /*public List<JewelryModelMedia> Medias { get; set; } = new();*/
        public Media? Thumbnail { get; set; }
        public List<Media>? Gallery { get; set; } = new();
        public JewelryModel() { }
        /*public static JewelryModel Create(ModelSpec modelSpec, JewelryModelId givenId = null)
        {
            return new JewelryModel()
            {
                Id = givenId is null ? JewelryModelId.Create() : givenId,
                Name = modelSpec.Name,
                CategoryId = JewelryModelCategoryId.Parse(modelSpec.CategoryId),
                Width = modelSpec.Width,
                Length = modelSpec.Length,
                IsEngravable = modelSpec.IsEngravable,
                IsRhodiumFinish = modelSpec.IsRhodiumFinish,
                BackType = modelSpec.BackType,
                ClaspType = modelSpec.ClaspType,
                ChainType = modelSpec.ChainType,
            };
        }*/
        public static JewelryModel Create(string name, JewelryModelCategoryId categoryId, float? width, float? length, bool isEngravable, bool isRhodiumFinish, BackType? backType, ClaspType? claspType, ChainType? chainType, JewelryModelId givenId = null)
        {
            return new JewelryModel()
            {
                Id = givenId is null ? JewelryModelId.Create() : givenId,
                Name = name,
                CategoryId = categoryId,
                Width = width,
                Length = length,
                IsEngravable = isEngravable,
                IsRhodiumFinish = isRhodiumFinish,
                BackType = backType,
                ClaspType = claspType,
                ChainType = chainType,
            };
        } 
    }
}

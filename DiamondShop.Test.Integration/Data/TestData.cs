using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Test.Integration.Data
{
    public static class TestData
    {
        public static List<SizeId> SizeIds = new()
        {
            SizeId.Parse("3"),
            SizeId.Parse("4"),
            SizeId.Parse("5"),
            SizeId.Parse("6"),
        };
        public static List<MetalId> MetalIds = new()
        {
            MetalId.Parse("1"),
            MetalId.Parse("2"),
            MetalId.Parse("3"),
            MetalId.Parse("4"),
        };
        public static JewelryModelCategory DefaultCategory = JewelryModelCategory.Create(
            "Test_Category", "this is a testing category, don't use", "", true, null, JewelryModelCategoryId.Parse("1"));
        #region DefaultRingModel
        public static JewelryModel DefaultRingModel = JewelryModel.Create(
            "Test_Model", DefaultCategory.Id, 1f, null, true, true,
            null, null, null, JewelryModelId.Parse("1"));

        public static List<MainDiamondReq> DefaultRingMainDiamondReqs = new()
        {
            MainDiamondReq.Create(DefaultRingModel.Id,SettingType.Prong,1,MainDiamondReqId.Parse($"1_1")),
        };
        public static List<MainDiamondShape> DefaultRingMainDiamondShapes = new()
        {
            MainDiamondShape.Create(DefaultRingMainDiamondReqs[0].Id,DiamondShapeId.Parse("1"),0.3f,2.5f),
            MainDiamondShape.Create(DefaultRingMainDiamondReqs[0].Id,DiamondShapeId.Parse("2"),0.3f,2.5f),
            MainDiamondShape.Create(DefaultRingMainDiamondReqs[0].Id,DiamondShapeId.Parse("3"),0.3f,2.5f),
        };
        public static List<SideDiamondReq> DefaultRingSideDiamondReqs = new()
        {
            SideDiamondReq.Create(DefaultRingModel.Id,DiamondShapeId.Parse("1"),Color.K, Color.D, Clarity.VS2, Clarity.IF, SettingType.Prong,SideDiamondReqId.Parse($"1_1")),
        };
        public static List<SideDiamondOpt> DefaultRingSideDiamondOpts = new()
        {
            SideDiamondOpt.Create(DefaultRingSideDiamondReqs[0].Id,0.05f, 25, SideDiamondOptId.Parse($"1_1_1")),
            SideDiamondOpt.Create(DefaultRingSideDiamondReqs[0].Id,0.25f, 10, SideDiamondOptId.Parse($"1_1_2")),
        };
        public static List<SizeMetal> DefaultRingSizeMetal = new()
        {
            SizeMetal.Create(DefaultRingModel.Id, MetalIds[0], SizeIds[0], 10),
            SizeMetal.Create(DefaultRingModel.Id, MetalIds[0], SizeIds[1], 12),
            SizeMetal.Create(DefaultRingModel.Id, MetalIds[0], SizeIds[2], 14),
        };
        #endregion

        #region Diamond
        public static Diamond DefaultDiamond = null;
        #endregion
    }
}

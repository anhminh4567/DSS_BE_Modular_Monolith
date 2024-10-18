using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Infrastructure.Databases;
using Microsoft.EntityFrameworkCore;

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
        //Already seeded
        public static JewelryModelCategoryId DefaultCategoryId = JewelryModelCategoryId.Parse("1");
        #region JewelryModel
        public static JewelryModel DefaultRingModel(string name, JewelryModelCategoryId categoryId, float? width, float? height, bool? isEngravable, bool? isRhodiumFinish, BackType? backType, ClaspType? claspType, ChainType? chainType, string id) => JewelryModel.Create(
            name, categoryId, width, height, isEngravable, isRhodiumFinish,
            backType, claspType, chainType, JewelryModelId.Parse(id));

        public static MainDiamondReq DefaultRingMainDiamondReq(JewelryModelId modelId, int quantity, string id) => MainDiamondReq.Create(modelId, SettingType.Prong, quantity, MainDiamondReqId.Parse($"{modelId.Value}_{id}"));
        public static List<MainDiamondShape> DefaultRingMainDiamondShapes(MainDiamondReqId mainDiamondId) => new()
        {
            MainDiamondShape.Create(mainDiamondId,DiamondShapeId.Parse("1"),0.3f,2.5f),
            MainDiamondShape.Create(mainDiamondId,DiamondShapeId.Parse("2"),0.3f,2.5f),
            MainDiamondShape.Create(mainDiamondId,DiamondShapeId.Parse("3"),0.3f,2.5f),
        };
        public static List<SideDiamondReq> DefaultRingSideDiamondReqs(JewelryModelId modelId) => new()
        {
            SideDiamondReq.Create(modelId,DiamondShapeId.Parse("1"),Color.K, Color.D, Clarity.VS2, Clarity.IF, SettingType.Prong,SideDiamondReqId.Parse($"{modelId}_1")),
        };
        public static List<SideDiamondOpt> DefaultRingSideDiamondOpts(SideDiamondReqId sideDiamondId) => new()
        {
            SideDiamondOpt.Create(sideDiamondId,0.05f, 25, SideDiamondOptId.Parse($"{sideDiamondId}_1")),
            SideDiamondOpt.Create(sideDiamondId,0.25f, 10, SideDiamondOptId.Parse($"{sideDiamondId}_2")),
        };
        public static List<SizeMetal> DefaultRingSizeMetal(JewelryModelId modelId) => new()
        {
            SizeMetal.Create(modelId, MetalIds[0], SizeIds[0], 10),
            SizeMetal.Create(modelId, MetalIds[0], SizeIds[1], 12),
            SizeMetal.Create(modelId, MetalIds[0], SizeIds[2], 14),
        };
        private static async Task SeedingModel(
            DiamondShopDbContext _context,
            JewelryModel model, List<MainDiamondReq>? mainDiamondReqs, List<MainDiamondShape>? mainDiamondShapes, List<SideDiamondReq>? sideDiamondReqs,
            List<SideDiamondOpt>? sideDiamondOpts, List<SizeMetal> sizeMetals)
        {
            _context.Set<JewelryModel>().Add(model);
            if(mainDiamondReqs != null) _context.Set<MainDiamondReq>().AddRange(mainDiamondReqs);
            if (mainDiamondShapes != null) _context.Set<MainDiamondShape>().AddRange(mainDiamondShapes);
            if(sideDiamondReqs != null) _context.Set<SideDiamondReq>().AddRange(sideDiamondReqs);
            if(sideDiamondOpts != null) _context.Set<SideDiamondOpt>().AddRange(sideDiamondOpts);
            _context.Set<SizeMetal>().AddRange(sizeMetals);
            await _context.SaveChangesAsync();
        }
        public static async Task<JewelryModel> SeedDefaultRingModel(DiamondShopDbContext _context)
        {
            var model = DefaultRingModel("Test_Default_Model", DefaultCategoryId, 1f, null, true, true, null, null, null, "1");
            var mainDiamonds = new List<MainDiamondReq>(){
                DefaultRingMainDiamondReq(model.Id,1,"1")
            };
            var mainDiamondShapes = new List<MainDiamondShape>();
            mainDiamonds.ForEach(p =>
            {
                var shapes = DefaultRingMainDiamondShapes(p.Id);
                mainDiamondShapes.AddRange(shapes);
                p.Shapes = shapes;
            });
            var sideDiamonds = DefaultRingSideDiamondReqs(model.Id);
            var sideDiamondOpts = new List<SideDiamondOpt>();
            sideDiamonds.ForEach(p =>
            {
                var opts = DefaultRingSideDiamondOpts(p.Id);
                sideDiamondOpts.AddRange(opts);
                p.SideDiamondOpts = opts;
            });
            var sizeMetals = DefaultRingSizeMetal(model.Id);
            await SeedingModel(_context, model, mainDiamonds, mainDiamondShapes, sideDiamonds, sideDiamondOpts, sizeMetals);
            model.MainDiamonds = mainDiamonds;
            model.SideDiamonds = sideDiamonds;
            model.SizeMetals = sizeMetals;
            return model;
        }
        public static async Task<JewelryModel> SeedMultiMainDiamondRingModel(DiamondShopDbContext _context)
        {
            var model = DefaultRingModel("Test_MultiMain_Model", DefaultCategoryId, 1f, null, true, true, null, null, null, "1");
            var mainDiamonds = new List<MainDiamondReq>(){
                DefaultRingMainDiamondReq(model.Id,1,"1"),
                DefaultRingMainDiamondReq(model.Id,2,"2"),
            };
            var mainDiamondShapes = new List<MainDiamondShape>();
            mainDiamonds.ForEach(p =>
            {
                var shapes = DefaultRingMainDiamondShapes(p.Id);
                mainDiamondShapes.AddRange(shapes);
                p.Shapes = shapes;
            });
            var sideDiamonds = DefaultRingSideDiamondReqs(model.Id);
            var sideDiamondOpts = new List<SideDiamondOpt>();
            sideDiamonds.ForEach(p =>
            {
                var opts = DefaultRingSideDiamondOpts(p.Id);
                sideDiamondOpts.AddRange(opts);
                p.SideDiamondOpts = opts;
            });
            var sizeMetals = DefaultRingSizeMetal(model.Id);
            await SeedingModel(_context, model, mainDiamonds, mainDiamondShapes, sideDiamonds, sideDiamondOpts, sizeMetals);
            model.MainDiamonds = mainDiamonds;
            model.SideDiamonds = sideDiamonds;
            model.SizeMetals = sizeMetals;
            return model;
        }
        public static async Task<JewelryModel> SeedNoDiamondRingModel(DiamondShopDbContext _context)
        {
            var model = DefaultRingModel("Test_NoDiamond_Model", DefaultCategoryId, 1f, null, true, true, null, null, null, "1");
            _context.Set<JewelryModel>().Add(model);
            var sizeMetals = DefaultRingSizeMetal(model.Id);
            await SeedingModel(_context, model, null, null, null, null, sizeMetals);
            model.SizeMetals = sizeMetals;
            return model;
        }
        public static async Task<JewelryModel> SeedDefaultNecklaceModel(DiamondShopDbContext _context)
        {
            var model = DefaultRingModel("Test_Necklace", DefaultCategoryId, null, 10f, false, false, null, ClaspType.Spring_Ring, ChainType.Rope, "1");
            var mainDiamonds = new List<MainDiamondReq>(){
                DefaultRingMainDiamondReq(model.Id,1,"1")
            };
            var mainDiamondShapes = new List<MainDiamondShape>();
            mainDiamonds.ForEach(p =>
            {
                var shapes = DefaultRingMainDiamondShapes(p.Id);
                mainDiamondShapes.AddRange(shapes);
                p.Shapes = shapes;
            });
            var sideDiamonds = DefaultRingSideDiamondReqs(model.Id);
            var sideDiamondOpts = new List<SideDiamondOpt>();
            sideDiamonds.ForEach(p =>
            {
                var opts = DefaultRingSideDiamondOpts(p.Id);
                sideDiamondOpts.AddRange(opts);
                p.SideDiamondOpts = opts;
            });
            var sizeMetals = DefaultRingSizeMetal(model.Id);
            await SeedingModel(_context, model, mainDiamonds, mainDiamondShapes, sideDiamonds, sideDiamondOpts, sizeMetals);
            model.MainDiamonds = mainDiamonds;
            model.SideDiamonds = sideDiamonds;
            model.SizeMetals = sizeMetals;
            return model;
        }

        #endregion

        #region Diamond
        static async Task SeedingDiamond(DiamondShopDbContext _context, Diamond diamond)
        {
            _context.Set<Diamond>().Add(diamond);
            await _context.SaveChangesAsync();
        }
        static async Task SeedingDiamonds(DiamondShopDbContext _context, List<Diamond> diamonds)
        {
            _context.Set<Diamond>().AddRange(diamonds);
            await _context.SaveChangesAsync();
        }
        public static async Task<Diamond> SeedDefaultDiamond(DiamondShopDbContext _context)
        {
            DiamondShape DefaultDiamondShape = DiamondShape.Create("Round", DiamondShapeId.Parse("1"));
            Diamond_4C DefaultDiamond4C = new Diamond_4C(Cut.Very_Good, Color.H, Clarity.VS1, 1, false);
            Diamond_Details DefaultDiamondDetail = new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.None, Culet.None);
            Diamond_Measurement DefaultDiamondMeasurement = new Diamond_Measurement(1f, 1f, 1f, "1x1");
            Diamond DefaultDiamond = Diamond.Create(DefaultDiamondShape, DefaultDiamond4C, DefaultDiamondDetail, DefaultDiamondMeasurement, 0m);
            await SeedingDiamond(_context, DefaultDiamond);
            return DefaultDiamond;
        }
        public static async Task<List<Diamond>> SeedDefaultDiamonds(DiamondShopDbContext _context, int quantity, string shapeId)
        {
            DiamondShape DefaultDiamondShape = DiamondShape.Create("Round", DiamondShapeId.Parse(shapeId));
            Diamond_4C DefaultDiamond4C = new Diamond_4C(Cut.Very_Good, Color.H, Clarity.VS1, 1, false);
            Diamond_Details DefaultDiamondDetail = new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.None, Culet.None);
            Diamond_Measurement DefaultDiamondMeasurement = new Diamond_Measurement(1f, 1f, 1f, "1x1");
            List<Diamond> DefaultDiamonds = new() { };
            for (int i = 0; i < quantity; i++)
            {
                DefaultDiamonds.Add(Diamond.Create(DefaultDiamondShape, DefaultDiamond4C, DefaultDiamondDetail, DefaultDiamondMeasurement, 0m));
            }
            await SeedingDiamonds(_context, DefaultDiamonds);
            return DefaultDiamonds;
        }

        #endregion


    }
}

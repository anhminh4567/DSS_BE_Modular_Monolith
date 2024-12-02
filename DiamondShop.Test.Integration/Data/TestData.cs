using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
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
        public static List<DiamondShapeId> DiamondShapeIds = new()
        {
            DiamondShapeId.Parse("1"),
            DiamondShapeId.Parse("2"),
            DiamondShapeId.Parse("3"),
            DiamondShapeId.Parse("4"),
            DiamondShapeId.Parse("5"),
            DiamondShapeId.Parse("6"),
            DiamondShapeId.Parse("7"),
            DiamondShapeId.Parse("8"),
            DiamondShapeId.Parse("9"),
            DiamondShapeId.Parse("10"),
        };
        //Already seeded
        public static JewelryModelCategoryId DefaultCategoryId = JewelryModelCategoryId.Parse("1");
        #region JewelryModel
        public static JewelryModel DefaultRingModel(string name, string code, JewelryModelCategoryId categoryId, float? width, float? height, bool? isEngravable, bool? isRhodiumFinish, BackType? backType, ClaspType? claspType, ChainType? chainType, string id) => JewelryModel.Create(
            name, code, categoryId, 0,width, height, isEngravable, isRhodiumFinish,
            backType, claspType, chainType, JewelryModelId.Parse(id));

        public static MainDiamondReq DefaultRingMainDiamondReq(JewelryModelId modelId, int quantity, string id) => MainDiamondReq.Create(modelId, SettingType.Prong, quantity, MainDiamondReqId.Parse($"{modelId.Value}_{id}"));
        public static List<MainDiamondShape> DefaultRingMainDiamondShapes(MainDiamondReqId mainDiamondId) => new()
        {
            MainDiamondShape.Create(mainDiamondId,DiamondShapeId.Parse("1"),0.3f,2.5f),
            MainDiamondShape.Create(mainDiamondId,DiamondShapeId.Parse("2"),0.3f,2.5f),
            MainDiamondShape.Create(mainDiamondId,DiamondShapeId.Parse("3"),0.3f,2.5f),
        };
        public static List<SideDiamondOpt> DefaultRingSideDiamondOpts(JewelryModelId modelId) => new()
        {
            SideDiamondOpt.Create(modelId,DiamondShapeId.Parse("1"),Color.K, Color.D, Clarity.VS2, Clarity.IF, SettingType.Prong,0.15f,15,false,SideDiamondOptId.Parse($"{modelId}_1")),
            SideDiamondOpt.Create(modelId,DiamondShapeId.Parse("1"),Color.F, Color.D, Clarity.VVS1, Clarity.IF, SettingType.Prong,0.25f,2,false,SideDiamondOptId.Parse($"{modelId}_2")),
            SideDiamondOpt.Create(modelId,DiamondShapeId.Parse("1"),Color.D, Color.D, Clarity.IF, Clarity.IF, SettingType.Prong,0.35f,5,false, SideDiamondOptId.Parse($"{modelId}_3")),
        };
        public static List<SizeMetal> DefaultRingSizeMetal(JewelryModelId modelId) => new()
        {
            SizeMetal.Create(modelId, MetalIds[0], SizeIds[0], 10),
            SizeMetal.Create(modelId, MetalIds[0], SizeIds[1], 12),
            SizeMetal.Create(modelId, MetalIds[0], SizeIds[2], 14),
        };
        private static async Task SeedingModel(
            DiamondShopDbContext _context,
            JewelryModel model, List<MainDiamondReq>? mainDiamondReqs, List<MainDiamondShape>? mainDiamondShapes, List<SideDiamondOpt>? sideDiamondOpts,
            List<SizeMetal> sizeMetals)
        {
            _context.Set<JewelryModel>().Add(model);
            if (mainDiamondReqs != null) _context.Set<MainDiamondReq>().AddRange(mainDiamondReqs);
            if (mainDiamondShapes != null) _context.Set<MainDiamondShape>().AddRange(mainDiamondShapes);
            if (sideDiamondOpts != null) _context.Set<SideDiamondOpt>().AddRange(sideDiamondOpts);
            _context.Set<SizeMetal>().AddRange(sizeMetals);
            await _context.SaveChangesAsync();
        }
        public static async Task<JewelryModel> SeedDefaultRingModel(DiamondShopDbContext _context, string modelId = "1")
        {
            var model = DefaultRingModel("Test Default Model", "TDM", DefaultCategoryId, 1f, null, true, true, null, null, null, modelId);
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
            var sideDiamondOpts = DefaultRingSideDiamondOpts(model.Id);
            var sizeMetals = DefaultRingSizeMetal(model.Id);
            await SeedingModel(_context, model, mainDiamonds, mainDiamondShapes, sideDiamondOpts, sizeMetals);
            model.MainDiamonds = mainDiamonds;
            model.SideDiamonds = sideDiamondOpts;
            model.SizeMetals = sizeMetals;
            return model;
        }
        public static async Task<JewelryModel> SeedMultiMainDiamondRingModel(DiamondShopDbContext _context)
        {
            var model = DefaultRingModel("Test MultiMain Model", "TMM", DefaultCategoryId, 1f, null, true, true, null, null, null, "1");
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
            var sideDiamondOpts = DefaultRingSideDiamondOpts(model.Id);
            var sizeMetals = DefaultRingSizeMetal(model.Id);
            await SeedingModel(_context, model, mainDiamonds, mainDiamondShapes, sideDiamondOpts, sizeMetals);
            model.MainDiamonds = mainDiamonds;
            model.SideDiamonds = sideDiamondOpts;
            model.SizeMetals = sizeMetals;
            return model;
        }
        public static async Task<JewelryModel> SeedSideDiamondOnlyRingModel(DiamondShopDbContext _context)
        {
            var model = DefaultRingModel("Test NoDiamond Model", "TNM", DefaultCategoryId, 1f, null, true, true, null, null, null, "1");
            _context.Set<JewelryModel>().Add(model);
            var sideDiamondOpts = DefaultRingSideDiamondOpts(model.Id);
            var sizeMetals = DefaultRingSizeMetal(model.Id);
            await SeedingModel(_context, model, null, null, sideDiamondOpts, sizeMetals);
            model.SizeMetals = sizeMetals;
            return model;
        }
        public static async Task<JewelryModel> SeedNoDiamondRingModel(DiamondShopDbContext _context)
        {
            var model = DefaultRingModel("Test NoDiamond Model", "TNM", DefaultCategoryId, 1f, null, true, true, null, null, null, "1");
            _context.Set<JewelryModel>().Add(model);
            var sizeMetals = DefaultRingSizeMetal(model.Id);
            await SeedingModel(_context, model, null, null, null, sizeMetals);
            model.SizeMetals = sizeMetals;
            return model;
        }
        public static async Task<JewelryModel> SeedDefaultNecklaceModel(DiamondShopDbContext _context)
        {
            var model = DefaultRingModel("Test_Necklace", "TN", DefaultCategoryId, null, 10f, false, false, null, ClaspType.Spring_Ring, ChainType.Rope, "1");
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
            var sideDiamondOpts = DefaultRingSideDiamondOpts(model.Id);
            var sizeMetals = DefaultRingSizeMetal(model.Id);
            await SeedingModel(_context, model, mainDiamonds, mainDiamondShapes, sideDiamondOpts, sizeMetals);
            model.MainDiamonds = mainDiamonds;
            model.SideDiamonds = sideDiamondOpts;
            model.SizeMetals = sizeMetals;
            return model;
        }

        #endregion

        #region Jewelry
        public static async Task<Jewelry> DefaultJewelry(string modelId, SizeId sizeId, MetalId metalId, string id)
            => Jewelry.Create(JewelryModelId.Parse(modelId), sizeId, metalId, 1f, "DEFAULT_JEWEL", givenId: JewelryId.Parse(id), status: Domain.Common.Enums.ProductStatus.Active);
        static async Task SeedingJewelry(DiamondShopDbContext _context, Jewelry jewelry)
        {
            _context.Set<Jewelry>().Add(jewelry);
            await _context.SaveChangesAsync();
        }
        public static async Task<Jewelry> SeedDefaultJewelry(DiamondShopDbContext _context, string modelId = "1", string jewelryId = "1")
        {
            var model = _context.Set<JewelryModel>().FirstOrDefault();
            if (model == null)
                model = await SeedDefaultRingModel(_context, modelId);
            var jewelry = await DefaultJewelry(model.Id.Value, SizeIds[0], MetalIds[0], jewelryId);
            var modelSideDiamond = model.SideDiamonds.FirstOrDefault();
            if (modelSideDiamond != null) jewelry.SideDiamond = JewelrySideDiamond.Create(modelSideDiamond);
            await SeedingJewelry(_context, jewelry);

            return jewelry;
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
        public static async Task<Diamond> SeedDefaultDiamond(DiamondShopDbContext _context, JewelryId? jewelryId = null)
        {
            DiamondShape DefaultDiamondShape = DiamondShape.Create("Round", DiamondShapeId.Parse("1"));
            Diamond_4C DefaultDiamond4C = new Diamond_4C(Cut.Excellent, Color.K, Clarity.FL, 1, false);
            Diamond_Details DefaultDiamondDetail = new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.None, Culet.None);
            Diamond_Measurement DefaultDiamondMeasurement = new Diamond_Measurement(1f, 1f, 1f, "1x1");
            Diamond DefaultDiamond = Diamond.Create(DefaultDiamondShape, DefaultDiamond4C, DefaultDiamondDetail, DefaultDiamondMeasurement, 0m, "abc");
            DefaultDiamond.JewelryId = jewelryId;
            await SeedingDiamond(_context, DefaultDiamond);
            return DefaultDiamond;
        }
        public static async Task<List<Diamond>> SeedDefaultDiamonds(DiamondShopDbContext _context, int quantity, string shapeId, JewelryId? jewelryId = null)
        {
            DiamondShape DefaultDiamondShape = DiamondShape.Create("Round", DiamondShapeId.Parse(shapeId));
            Diamond_4C DefaultDiamond4C = new Diamond_4C(Cut.Very_Good, Color.H, Clarity.VS1, 1, false);
            Diamond_Details DefaultDiamondDetail = new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.None, Culet.None);
            Diamond_Measurement DefaultDiamondMeasurement = new Diamond_Measurement(1f, 1f, 1f, "1x1");
            List<Diamond> DefaultDiamonds = new() { };
            for (int i = 0; i < quantity; i++)
            {
                var diamond = Diamond.Create(DefaultDiamondShape, DefaultDiamond4C, DefaultDiamondDetail, DefaultDiamondMeasurement, 0m,$"{i}");
                diamond.JewelryId = jewelryId;
                DefaultDiamonds.Add(diamond);
            }
            await SeedingDiamonds(_context, DefaultDiamonds);
            return DefaultDiamonds;
        }

        #endregion

        #region DiamondCriteria
        public static DiamondCriteria DefaultDiamondCriteria( bool isLab)
        => DiamondCriteria.Create( 0f, 3f, DiamondShape.ROUND);//Cut? cut, Clarity clarity, Color color,
        static async Task SeedingCriteria(DiamondShopDbContext _context, DiamondCriteria criteria)
        {
            _context.Set<DiamondCriteria>().AddRange(criteria);
            await _context.SaveChangesAsync();
        }
        public static async Task<DiamondCriteria> SeedDefaultDiamondCriteria(DiamondShopDbContext _context, bool isLab)//, Cut? cut, Clarity clarity, Color color
        {
            var criteria = DefaultDiamondCriteria( isLab);//cut, clarity, color
            await SeedingCriteria(_context, criteria);
            return criteria;
        }
        //public static async Task<DiamondCriteria> SeedSideDiamondCriteria(DiamondShopDbContext _context, float caratFrom, float caratTo, bool isLab)
        //{
        //    var criteria = DiamondCriteria.CreateSideDiamondCriteria()
        //    await SeedingCriteria(_context, criteria);
        //    return criteria;
        //}
        #endregion
        #region DiamondPrice
        public static DiamondPrice DefaultDiamondPrice(DiamondShapeId shapeId, DiamondCriteriaId criteriaId, bool isLab,Cut? cut, Clarity clarity, Color color) =>
            DiamondPrice.Create(shapeId, criteriaId, 100_000_000M, isLab,cut,color,clarity);
        public static DiamondPrice DefaultSideDiamondPrice(DiamondCriteriaId criteriaId, Cut? cut, Clarity clarity, Color color) =>
            DiamondPrice.CreateSideDiamondPrice(criteriaId, 100_000_000M, false, DiamondShape.ANY_SHAPES, null , color,clarity );
        static async Task SeedingPrice(DiamondShopDbContext _context, DiamondPrice price)
        {
            _context.Set<DiamondPrice>().AddRange(price);
            await _context.SaveChangesAsync();
        }
        public static async Task<DiamondPrice> SeedDefaultDiamondPrice(DiamondShopDbContext _context, DiamondShapeId shapeId, DiamondCriteriaId criteriaId, bool isLab, Cut? cut, Clarity clarity, Color color)
        {
            var price = DefaultDiamondPrice(shapeId, criteriaId, isLab,cut,clarity,color);
            await SeedingPrice(_context, price);
            return price;
        }
        public static async Task<DiamondPrice> SeedSideDiamondPrice(DiamondShopDbContext _context, DiamondCriteriaId criteriaId, Cut? cut, Clarity clarity, Color color)
        {
            var price = DefaultSideDiamondPrice(criteriaId,cut,clarity,color);
            await SeedingPrice(_context, price);
            return price;
        }
        #endregion
        #region Account
        public static Account DefaultCustomer(FullName fullname, string email, string identityId, List<AccountRole> roles) =>
            Account.CreateBaseCustomer(fullname, email, identityId, roles);
        public static Account DefaultStaff(FullName fullname, string email, string identityId, List<AccountRole> roles) =>
           Account.CreateBaseStaff(fullname, email, identityId, roles);
        public static Account DefaultManager(FullName fullname, string email, string identityId, List<AccountRole> roles) =>
           Account.CreateBaseManager(fullname, email, identityId, roles);
        public static Account DefaultDeliverer(FullName fullname, string email, string identityId, List<AccountRole> roles) =>
           Account.CreateBaseDeliverer(fullname, email, identityId, roles);
        static async Task SeedingAccount(DiamondShopDbContext _context, Account account)
        {
            _context.Set<Account>().Add(account);
            await _context.SaveChangesAsync();
        }
        public static async Task<Account> SeedDefaultCustomer(DiamondShopDbContext _context, IAuthenticationService _authentication)
        {
            var roles = await _context.Set<AccountRole>().ToListAsync();
            string email = "abc@gmail.com";
            FullName username = FullName.parse("", "User_A");
            var register = await _authentication.Register(email, "123", username, true);
            var user = DefaultCustomer(username, email, register.Value, roles);
            await SeedingAccount(_context, user);
            return user;
        }
        public static async Task<Account> SeedDefaultStaff(DiamondShopDbContext _context, IAuthenticationService _authentication)
        {
            var roles = await _context.Set<AccountRole>().ToListAsync();
            string email = "abc@staff.com";
            FullName username = FullName.parse("", "Staff_A");
            var register = await _authentication.Register(email, "123", username, true);
            var user = DefaultStaff(username, email, register.Value, roles);
            await SeedingAccount(_context, user);
            return user;
        }
        public static async Task<Account> SeedDefaultManager(DiamondShopDbContext _context, IAuthenticationService _authentication)
        {
            var roles = await _context.Set<AccountRole>().ToListAsync();
            string email = "abc@manager.com";
            FullName username = FullName.parse("", "Manager_A");
            var register = await _authentication.Register(email, "123", username, true);
            var user = DefaultManager(username, email, register.Value, roles);
            await SeedingAccount(_context, user);
            return user;
        }
        public static async Task<Account> SeedDefaultDeliverer(DiamondShopDbContext _context, IAuthenticationService _authentication)
        {
            var roles = await _context.Set<AccountRole>().ToListAsync();
            string email = "abc@deliverer.com";
            FullName username = FullName.parse("", "Deliverer_A");
            var register = await _authentication.Register(email, "123", username, true);
            var user = DefaultDeliverer(username, email, register.Value, roles);
            await SeedingAccount(_context, user);
            return user;
        }
        #endregion
    }
}

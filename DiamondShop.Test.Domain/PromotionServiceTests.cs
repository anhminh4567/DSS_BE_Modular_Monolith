using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using OpenQA.Selenium.DevTools.V127.Audits;

namespace DiamondShop.Test.Domain
{
    public class PromotionServiceTests
    {
        private List<DiamondShape> _diamondShapes;
        public PromotionServiceTests()
        {
            _diamondShapes = new List<DiamondShape> {
                DiamondShape.Create("Round",DiamondShapeId.Parse(1.ToString())),
                DiamondShape.Create("Princess",DiamondShapeId.Parse(2.ToString())),
                DiamondShape.Create("Cushion",DiamondShapeId.Parse(3.ToString())),
                DiamondShape.Create("Emerald",DiamondShapeId.Parse(4.ToString())),
                DiamondShape.Create("Oval",DiamondShapeId.Parse(5.ToString())),
                DiamondShape.Create("Radiant",DiamondShapeId.Parse(6.ToString())),
                DiamondShape.Create("Asscher",DiamondShapeId.Parse(7.ToString())),
                DiamondShape.Create("Marquise",DiamondShapeId.Parse(8.ToString())),
                DiamondShape.Create("Heart",DiamondShapeId.Parse(9.ToString())),
                DiamondShape.Create("Pear",DiamondShapeId.Parse(10.ToString()))
            };
        }

        [Fact(DisplayName ="exclude qualifier")]
        public void ApplyPromotionOnCartModel_ShouldApplyPromotion_IsExcludeQualifier()
        {
            // Arrange
            List<CartItem> userCartItemWithDiamonds = new List<CartItem>()
            {
                CartItem.CreateDiamond(DiamondId.Create(),null),
                CartItem.CreateDiamond(DiamondId.Create(),null),
                CartItem.CreateDiamond(DiamondId.Create(),null),
            };
            var shapesIds = _diamondShapes.Select(s => s.Id).ToList();
            var promotionRequirement = PromoReq.CreateDiamondRequirement("test", Operator.Equal_Or_Larger, false, null, 1, DiamondOrigin.Lab, 0, 10, Clarity.S12, Clarity.FL, Cut.Good, Cut.Ideal, Color.I, Color.D, _diamondShapes);
            var promotionGift = Gift.CreateDiamond("test", null, UnitType.Percent, 20, 1, shapesIds, DiamondOrigin.Lab, 0, 10, Clarity.S12, Clarity.FL, Cut.Good, Cut.Ideal, Color.I, Color.D);
            var promotion = Promotion.Create("test", "test","test", DateTime.UtcNow, DateTime.UtcNow.AddDays(50), 1, true, RedemptionMode.Single);
            promotionRequirement.PromotionId = promotion.Id;
            promotionGift.PromotionId = promotion.Id;

            promotion.AddRequirement(promotionRequirement);
            promotion.AddGift(promotionGift);
            promotion.SetActive();

            Diamond diamond1 = Diamond.Create(_diamondShapes[0], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.5f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever"), 1);
            Diamond diamond2 = Diamond.Create(_diamondShapes[1], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.3f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever 2"),1);

            CartModel userCartModel = new CartModel();
            CartProduct product1 = new CartProduct() { Diamond = diamond1, ReviewPrice = new CheckoutPrice() { DefaultPrice = 1000 } };
            CartProduct product2 = new CartProduct() { Diamond = diamond2, ReviewPrice = new CheckoutPrice() { DefaultPrice = 2000 } };
            userCartModel.Products.Add(product1);
            userCartModel.Products.Add(product2);

            var promotionService = new PromotionService();
            // Act
            var result = promotionService.ApplyPromotionOnCartModel(userCartModel, promotion);

            // Assert
            Assert.True(result.IsSuccess);
            var promo = userCartModel.Promotion;
            Assert.Equal(userCartModel.Products[promo.RequirementProductsIndex[0]].CartProductId, product1.CartProductId);
            Assert.Equal(userCartModel.Products[promo.GiftProductsIndex[0]].CartProductId, product2.CartProductId);
        }
        [Fact(DisplayName = "include qualifier")]
        public void ApplyPromotionOnCartModel_ShouldApplyPromotion_NOTExcludeQualifier()
        {
            // Arrange
            var shapesIds = _diamondShapes.Select(s => s.Id).ToList();
            var promotionRequirement = PromoReq.CreateDiamondRequirement("test", Operator.Equal_Or_Larger, false, null, 1, DiamondOrigin.Lab, 0, 10, Clarity.S12, Clarity.FL, Cut.Good, Cut.Ideal, Color.I, Color.D, _diamondShapes);
            var promotionGift = Gift.CreateDiamond("test", null, UnitType.Percent, 20, 1, shapesIds, DiamondOrigin.Lab, 0, 10, Clarity.S12, Clarity.FL, Cut.Good, Cut.Ideal, Color.I, Color.D);
            var promotion = Promotion.Create("test", "test", "test", DateTime.UtcNow, DateTime.UtcNow.AddDays(50), 1, false, RedemptionMode.Single);
            promotionRequirement.PromotionId = promotion.Id;
            promotionGift.PromotionId = promotion.Id;

            promotion.AddRequirement(promotionRequirement);
            promotion.AddGift(promotionGift);
            promotion.SetActive();

            Diamond diamond1 = Diamond.Create(_diamondShapes[0], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.5f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever"), 1);
            Diamond diamond2 = Diamond.Create(_diamondShapes[1], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.3f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever 2"), 1);

            CartModel userCartModel = new CartModel();
            CartProduct product1 = new CartProduct() { Diamond = diamond1, ReviewPrice = new CheckoutPrice() { DefaultPrice = 1100 } };
            CartProduct product2 = new CartProduct() { Diamond = diamond2, ReviewPrice = new CheckoutPrice() { DefaultPrice = 2000 } };
            userCartModel.Products.Add(product1);
            userCartModel.Products.Add(product2);
            var promotionService = new PromotionService();
            // Act
            var result = promotionService.ApplyPromotionOnCartModel(userCartModel, promotion);

            // Assert
            Assert.True(result.IsSuccess);
            var promo = userCartModel.Promotion;
            Assert.Equal(userCartModel.Products[promo.RequirementProductsIndex[0]].CartProductId, product1.CartProductId);
            Assert.Equal(userCartModel.Products[promo.GiftProductsIndex[0]].CartProductId, product1.CartProductId);
            Assert.Equal(userCartModel.Products[promo.GiftProductsIndex[0]].CartProductId, userCartModel.Products[promo.RequirementProductsIndex[0]].CartProductId);
            Assert.Equal(promo.GiftProductsIndex.Count, promo.RequirementProductsIndex.Count);

        }
        [Fact()]
        public void ApplyPromotionOnCartModel_ProductRequirement_OrderGift_ShouldApplyPromotion() {
            // Arrange
            var shapesIds = _diamondShapes.Select(s => s.Id).ToList();
            int amountOfDiamond = 2;
            var giftOrderPercent = 20;
            var diamond1Price = 11000000;
            var diamond2Price = 15000000;
            var expectedPriceRequirement = diamond1Price + diamond2Price - 5000000;
            var promotionRequirement = PromoReq.CreateDiamondRequirement("test", Operator.Equal_Or_Larger, true, expectedPriceRequirement, null, DiamondOrigin.Lab, 0, 10, Clarity.S12, Clarity.FL, Cut.Good, Cut.Ideal, Color.I, Color.D, _diamondShapes);
            var promotionGift = Gift.CreateOrder("test",UnitType.Percent,giftOrderPercent);//Gift.CreateDiamond("test", null, UnitType.Percent, 20, 1, shapesIds, DiamondOrigin.Lab, 0, 10, Clarity.S12, Clarity.FL, Cut.Good, Cut.Excelent, Color.I, Color.D);
            var promotion = Promotion.Create("test", "test", "test", DateTime.UtcNow, DateTime.UtcNow.AddDays(50), 1, false, RedemptionMode.Single);
            promotionRequirement.PromotionId = promotion.Id;
            promotionGift.PromotionId = promotion.Id;

            promotion.AddRequirement(promotionRequirement);
            promotion.AddGift(promotionGift);
            promotion.SetActive();

            Diamond diamond1 = Diamond.Create(_diamondShapes[0], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.5f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever"), 1);
            Diamond diamond2 = Diamond.Create(_diamondShapes[1], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.3f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever 2"), 1);

            CartModel userCartModel = new CartModel();
            CartProduct product1 = new CartProduct() { Diamond = diamond1, ReviewPrice = new CheckoutPrice() { DefaultPrice = diamond1Price } };
            CartProduct product2 = new CartProduct() { Diamond = diamond2, ReviewPrice = new CheckoutPrice() { DefaultPrice = diamond2Price } };
            userCartModel.Products.Add(product1);
            userCartModel.Products.Add(product2);
            userCartModel.OrderPrices.DefaultPrice = diamond1Price + diamond2Price;

            var promotionService = new PromotionService();

            // Act
            var result = promotionService.ApplyPromotionOnCartModel(userCartModel, promotion);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userCartModel.OrderPrices.DefaultPrice,  diamond2Price + diamond1Price );
            Assert.Equal(userCartModel.OrderPrices.PromotionAmountSaved, decimal.Multiply((diamond2Price + diamond1Price), (20m/100m)));
            Assert.Equal(userCartModel.OrderPrices.FinalPrice, diamond2Price + diamond1Price - decimal.Multiply((diamond2Price + diamond1Price), (20m / 100m)));

        }
        [Fact()]
        public void ApplyPromotionOnCartModel_2Requirement_1Gift_ShouldApplyPromotion()
        {
            // Arrange
            var shapesIds = _diamondShapes.Select(s => s.Id).ToList();

            var diamontRequirementAmount = 2;
       
            var diamond1Price = 11000000;
            var diamond2Price = 15000000;
            var expectedOrderPriceRequirement = diamond1Price + diamond2Price - 5000000;
            var giftOrderAmountReduced =  10000000;
            var expectedFinalPrice = diamond1Price + diamond2Price - giftOrderAmountReduced;

            var promotionRequirement1= PromoReq.CreateDiamondRequirement("test", Operator.Equal_Or_Larger, false, null, diamontRequirementAmount, DiamondOrigin.Lab, 0, 10, Clarity.S12, Clarity.FL, Cut.Good, Cut.Ideal, Color.I, Color.D, _diamondShapes);
            var promotionRequirement2 = PromoReq.CreateOrderRequirement("test", Operator.Equal_Or_Larger, expectedOrderPriceRequirement);

            var promotionGift = Gift.CreateOrder("test", UnitType.Fix_Price, giftOrderAmountReduced);//Gift.CreateDiamond("test", null, UnitType.Percent, 20, 1, shapesIds, DiamondOrigin.Lab, 0, 10, Clarity.S12, Clarity.FL, Cut.Good, Cut.Excelent, Color.I, Color.D);
            var promotion = Promotion.Create("test", "test", "test", DateTime.UtcNow, DateTime.UtcNow.AddDays(50), 1, false, RedemptionMode.Single);
            promotionRequirement1.PromotionId = promotion.Id;
            promotionRequirement2.PromotionId = promotion.Id;

            promotionGift.PromotionId = promotion.Id;

            promotion.AddRequirement(promotionRequirement1);
            promotion.AddRequirement(promotionRequirement2);

            promotion.AddGift(promotionGift);
            promotion.SetActive();

            Diamond diamond1 = Diamond.Create(_diamondShapes[0], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.5f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever"), 1);
            Diamond diamond2 = Diamond.Create(_diamondShapes[1], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.3f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever 2"), 1);

            CartModel userCartModel = new CartModel();
            CartProduct product1 = new CartProduct() { Diamond = diamond1, ReviewPrice = new CheckoutPrice() { DefaultPrice = diamond1Price } };
            CartProduct product2 = new CartProduct() { Diamond = diamond2, ReviewPrice = new CheckoutPrice() { DefaultPrice = diamond2Price } };
            userCartModel.Products.Add(product1);
            userCartModel.Products.Add(product2);
            userCartModel.OrderPrices.DefaultPrice = diamond1Price + diamond2Price;

            var promotionService = new PromotionService();

            // Act
            var result = promotionService.ApplyPromotionOnCartModel(userCartModel, promotion);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userCartModel.OrderPrices.DefaultPrice, diamond2Price + diamond1Price);
            Assert.Equal(userCartModel.OrderPrices.PromotionAmountSaved, giftOrderAmountReduced);
            Assert.Equal(userCartModel.OrderPrices.FinalPrice, expectedFinalPrice);

        }

    }
}
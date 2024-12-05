using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;
using DiamondShop.Domain.BusinessRules;

namespace DiamondShop.Test.Domain
{
    public class DiscountServiceTest
    {
        private readonly DiscountService _discountService;
        private readonly IDiamondServices _diamondServices;
        private List<DiamondShape> _diamondShapes;
        public DiscountServiceTest()
        {
            //_diamondServices = new DiamondServices();
            _discountService = new DiscountService(_diamondServices) ;
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
        [Fact]
        public void ApplyDiscount_WithValidDiscount_ShouldReturnSuccess()
        {
            // Arrange
            List<CartItem> userCartItemWithDiamonds = new List<CartItem>()
            {
                CartItem.CreateDiamond(DiamondId.Create(),null),
                CartItem.CreateDiamond(DiamondId.Create(),null),
                CartItem.CreateDiamond(DiamondId.Create(),null),
            };
            var shapesIds = _diamondShapes.Select(s => s.Id).ToList();
            var discountRequirement = PromoReq.CreateDiamondRequirement("test discount", Operator.Equal_Or_Larger, false, null, 1, DiamondOrigin.Lab, 0, 10, Clarity.S12, Clarity.FL, Cut.Good, Cut.Excellent, Color.I, Color.D, _diamondShapes);
            var discount = Discount.Create("test discount", DateTime.UtcNow, DateTime.UtcNow.AddDays(20),20,"whateveryouwant");
            discountRequirement.DiscountId = discount.Id;
            discount.SetRequirement(discountRequirement);
            discount.SetActive();

            Diamond diamond1 = Diamond.Create(_diamondShapes[0], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.5f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever"),1, "asdfasdf");
            Diamond diamond2 = Diamond.Create(_diamondShapes[1], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.3f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever 2"), 1, "asdfasdfsdf");
            var diamond1Price = 13567000;
            var diamond2Price = 17567000;

            CartModel userCartModel = new CartModel();
            CartProduct product1 = new CartProduct() { Diamond = diamond1, ReviewPrice = new CheckoutPrice() { DefaultPrice = diamond1Price } };
            CartProduct product2 = new CartProduct() { Diamond = diamond2, ReviewPrice = new CheckoutPrice() { DefaultPrice = diamond2Price } };
            userCartModel.Products.Add(product1);
            userCartModel.Products.Add(product2);

            var product1ExpectedSavedAmount = MoneyVndRoundUpRules.RoundAmountFromDecimal( (int)Math.Ceiling((diamond1Price * 20d) / 100));
            var product2ExpectedSavedAmount = MoneyVndRoundUpRules.RoundAmountFromDecimal( (int)Math.Ceiling((diamond2Price * 20d) / 100));

            // Act
            var result = _discountService.ApplyDiscountOnCartModel(userCartModel, discount);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(userCartModel.DiscountsApplied.Exists(d => d.Id == discount.Id));
            Assert.Equal(userCartModel.Products.Where(p => p.IsHavingDiscount).Count(), userCartModel.Products.Count());
            Assert.Equal(userCartModel.Products.First(p => p.CartProductId == product1.CartProductId).ReviewPrice.DiscountAmountSaved, product1ExpectedSavedAmount);
            Assert.Equal(userCartModel.Products.First(p => p.CartProductId == product2.CartProductId).ReviewPrice.DiscountAmountSaved, product2ExpectedSavedAmount);
            //Assert.Equal(userCartModel.OrderPrices.DiscountAmountSaved, product1ExpectedSavedAmount + product2ExpectedSavedAmount);

        }
        [Fact]
        public void ApplyPromotionDiscount_WithValidGift_ShouldReturnSuccess()
        {
            // Arrange
            List<CartItem> userCartItemWithDiamonds = new List<CartItem>()
            {
                CartItem.CreateDiamond(DiamondId.Create(),null),
                CartItem.CreateDiamond(DiamondId.Create(),null),
                CartItem.CreateDiamond(DiamondId.Create(),null),
            };
            var shapesIds = _diamondShapes.Select(s => s.Id).ToList();
            var promotionGift = Gift.CreateDiamond("test", null, UnitType.Percent, 50, 1, shapesIds, DiamondOrigin.Both, 0, 10, Clarity.S12, Clarity.FL, Cut.Good, Cut.Excellent, Color.I, Color.D);
            var promotion = Promotion.Create("test", "test", "test", DateTime.UtcNow, DateTime.UtcNow.AddDays(50), 1, true, RedemptionMode.Single);
            promotionGift.PromotionId = promotion.Id;
            promotion.AddGift(promotionGift);
            promotion.Status = Status.Active;
            promotion.ApplyLevel = PromotionApplyLevel.On_Item;
            Diamond diamond1 = Diamond.Create(_diamondShapes[0], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.5f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever"), 1, "asdfasdf");
            Diamond diamond2 = Diamond.Create(_diamondShapes[1], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.3f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever 2"), 1, "asdfasdfsdf");
            var diamond1Price = 13567000;
            var diamond2Price = 17567000;

            CartModel userCartModel = new CartModel();
            CartProduct product1 = new CartProduct() { Diamond = diamond1, ReviewPrice = new CheckoutPrice() { DefaultPrice = diamond1Price } };
            CartProduct product2 = new CartProduct() { Diamond = diamond2, ReviewPrice = new CheckoutPrice() { DefaultPrice = diamond2Price } };
            userCartModel.Products.Add(product1);
            userCartModel.Products.Add(product2);

            var product1ExpectedSavedAmount = MoneyVndRoundUpRules.RoundAmountFromDecimal((int)Math.Ceiling((diamond1Price * 50d) / 100));
            var product2ExpectedSavedAmount = MoneyVndRoundUpRules.RoundAmountFromDecimal((int)Math.Ceiling((diamond2Price * 50d) / 100));

            // Act
            var result = _discountService.ApplyPromotionTypeDiscountOnCartModel(userCartModel, promotion, PromotionRule.Default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(userCartModel.DiscountPromoApplied.Exists(d => d.Id == promotion.Id));
            Assert.Equal(userCartModel.Products.Where(p => p.IsHavingPromoDiscount).Count(), userCartModel.Products.Count());
            Assert.Equal(userCartModel.Products.First(p => p.CartProductId == product1.CartProductId).ReviewPrice.DiscountAmountSaved, product1ExpectedSavedAmount);
            Assert.Equal(userCartModel.Products.First(p => p.CartProductId == product2.CartProductId).ReviewPrice.DiscountAmountSaved, product2ExpectedSavedAmount);
            //Assert.Equal(userCartModel.OrderPrices.DiscountAmountSaved, product1ExpectedSavedAmount + product2ExpectedSavedAmount);
        }
    }
    

}

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
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;

namespace DiamondShop.Test.Domain
{
    public class PromotionServiceTests
    {
        [Fact]
        public void ApplyPromotionOnCartModel_ShouldApplyPromotion_WhenRequirementsMet()
        {
            // Arrange
            List<CartItem> userCartItemWithDiamonds = new List<CartItem>()
            {
                CartItem.CreateDiamond(DiamondId.Create(),null),
                CartItem.CreateDiamond(DiamondId.Create(),null),
                CartItem.CreateDiamond(DiamondId.Create(),null),
            };
            var shapes = new List<DiamondShape> {
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
            var shapesIds = shapes.Select(s => s.Id).ToList();
            var promotionRequirement = PromoReq.CreateDiamondRequirement("test", Operator.Equal_Or_Larger, false, null, 1, DiamondOrigin.Lab, 0, 10, Clarity.S12, Clarity.FL, Cut.Good, Cut.Astor_Ideal, Color.I, Color.D,shapes);
            var promotionGift = Gift.CreateDiamond("test", null,UnitType.Percent, 20, 1, shapesIds, DiamondOrigin.Lab, 0, 10, Clarity.S12, Clarity.FL, Cut.Good, Cut.Astor_Ideal, Color.I, Color.D);
            var promotion = Promotion.Create("test","test",DateTime.UtcNow,DateTime.UtcNow.AddDays(50),1,false,RedemptionMode.Single);
            promotion.AddRequirement(promotionRequirement);
            promotion.AddGift(promotionGift);

            Diamond diamond1 = Diamond.Create(shapes[0], new Diamond_4C(Cut.Very_Good,Color.I,Clarity.VVS1,0.5f,true),new Diamond_Details(Polish.Good,Symmetry.Good,Girdle.Medium,Fluorescence.Medium,Culet.Medium),true,new Diamond_Measurement(2f,22f,2f,"whatever"));
            Diamond diamond2 = Diamond.Create(shapes[1], new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.3f, true), new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), true, new Diamond_Measurement(2f, 22f, 2f, "whatever 2"));

            CartModel userCartModel = new CartModel();
            CartProduct product1 = new CartProduct() { Diamond = diamond1 ,ReviewPrice = new CheckoutPrice() { DefaultPrice = 1000 } };
            CartProduct product2 = new CartProduct() { Diamond = diamond2, ReviewPrice = new CheckoutPrice() { DefaultPrice = 2000 } };
            userCartModel.Products.Add(product1);
            userCartModel.Products.Add(product2);

            //userCartModel.Products.Add();

            var promotionService = new PromotionService();
            // Act
            var result = promotionService.ApplyPromotionOnCartModel(userCartModel, promotion);
            
            // Assert
            Assert.True(result.IsSuccess);
            //Assert.Equal(promotion, cartModel.Promotion.Promotion);
        }

        [Fact(Skip = "not now")]
        public void ApplyPromotionOnCartModel_ShouldNotApplyPromotion_WhenRequirementsNotMet()
        {
            // Arrange
            var cartModel = new CartModel();
            var promotion = new Promotion();
            var promotionService = new PromotionService();

            // Act
            var result = promotionService.ApplyPromotionOnCartModel(cartModel, promotion);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Null(cartModel.Promotion.Promotion);
        }
    }
}
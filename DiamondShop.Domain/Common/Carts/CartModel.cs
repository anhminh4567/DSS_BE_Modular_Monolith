using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds;

namespace DiamondShop.Domain.Common.Carts
{
    public class CartModel
    {
        public CartModelPromotion Promotion { get; set; } = new CartModelPromotion();
        public List<Discount> DiscountsApplied { get; set; } = new();
        public CheckoutPrice OrderPrices { get; set; } = new() { DefaultPrice = 0 };
        public ShippingPrice ShippingPrice { get; set; } = new();
        public CartModelCounter OrderCounter { get; set; } = new();
        public CartModelValidation OrderValidation { get; set; } = new();
        public List<CartProduct> Products { get; set; } = new();
        public void SetErrorMessages()
        {
            OrderValidation.SetErrorMessageInTheEnd(ShippingPrice);
        }
        public void SetOrderShippingPrice(ShippingPrice shipping)
        {
            ShippingPrice = shipping;
            OrderPrices.DefaultPrice += shipping.FinalPrice;
        }
        public void SetOrderPrice()
        {
            foreach (var product in Products)
            {
                if (product.IsValid)
                {
                    OrderPrices.DiscountAmountSaved += product.ReviewPrice.DiscountAmountSaved;
                    // promotion amount saved is set by the prmotion service, since only 1 promotion is applied at a time
                    // and the promotion might include orderPromotion, which again, might affect the final price, 
                    // so the promotion amount saved is set here will be WRONG
                    //orderPrice.PromotionAmountSaved += product.ReviewPrice.PromotionAmountSaved;
                }
            }
        }
    }

}

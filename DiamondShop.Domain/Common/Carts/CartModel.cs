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
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.RoleAggregate;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace DiamondShop.Domain.Common.Carts
{

    public class CartModel
    {
        public CartModelPromotion Promotion { get; set; } = new CartModelPromotion();
        public List<Discount> DiscountsApplied { get; set; } = new();
        public CartModelPrice OrderPrices { get; set; } = new() { DefaultPrice = 0 };
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

            //OrderPrices.DefaultPrice += shipping.FinalPrice; 

            OrderPrices.TotalShippingPrice += shipping.FinalPrice;
        }
        public void SetWarrantyTotalPrice()
        {
            foreach(var prod in Products)
            {
                if (prod.CurrentWarrantyApplied != null)
                {
                    OrderPrices.TotalWarrantyPrice += prod.CurrentWarrantyPrice;
                }
            }
        }
        public void SetUserRankDiscount(PromotionRule rankDiscountRules, Account? userAccount)
        {
            if(userAccount!= null && userAccount.Roles != null)
            {
                var goldUserId = AccountRole.CustomerGold.Id;
                var silverUserId = AccountRole.CustomerSilver.Id;
                var bronzeUserId = AccountRole.CustomerBronze.Id;
                if (userAccount.Roles.Any(x => x.Id == goldUserId))
                    SetUserRankDiscountPercent(rankDiscountRules.GoldUserDiscountPercent);
                else if (userAccount.Roles.Any(x => x.Id == silverUserId))
                    SetUserRankDiscountPercent(rankDiscountRules.SilverUserDiscountPercent);
                else if (userAccount.Roles.Any(x => x.Id == bronzeUserId))
                    SetUserRankDiscountPercent(rankDiscountRules.BronzeUserDiscountPercent);
            }
        }
        private void SetUserRankDiscountPercent(decimal discountPercent)
        {
            OrderPrices.UserRankDiscountPercent = discountPercent;
            OrderPrices.UserRankDiscountAmount = MoneyVndRoundUpRules.RoundAmountFromDecimal(OrderPrices.OrderPriceExcludeShipAndWarranty * ((decimal)discountPercent / 100m)) ;
            OrderPrices.OrderAmountSaved += OrderPrices.UserRankDiscountAmount;
        }
        public static CartModel? CloneCart(CartModel cartModelTobeCloned)
        {
            var serializedObj = JsonConvert.SerializeObject(cartModelTobeCloned,new JsonSerializerSettings() 
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
            return JsonConvert.DeserializeObject<CartModel>(serializedObj);
        }
    }
}

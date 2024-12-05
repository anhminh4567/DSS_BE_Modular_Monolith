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
using DiamondShop.Domain.Common.Carts.ErrorMessages;
using DiamondShop.Domain.Models.Promotions;

namespace DiamondShop.Domain.Common.Carts
{

    public class CartModel
    {
        public CartModelPromotion Promotion { get; set; } = new CartModelPromotion();
        public List<Discount> DiscountsApplied { get; set; } = new();
        [JsonIgnore]
        public List<Promotion> DiscountPromoApplied{ get; set; } = new();
        public CartModelPrice OrderPrices { get; set; } = new() { DefaultPrice = 0 };
        public ShippingPrice ShippingPrice { get; set; } = new();
        public CartModelCounter OrderCounter { get; set; } = new();
        public CartModelValidation OrderValidation { get; set; } = new();
        public List<CartProduct> Products { get; set; } = new();
        public Account? Account { get; set; }
        public void SetErrorMessages()
        {
            OrderValidation.SetErrorMessageInTheEnd(ShippingPrice);
        }
        public void ValidateCartRules(CartModelRules cartModelRules)
        {
            if(cartModelRules.MaxItemPerCart < Products.Count)
                OrderValidation.SetErrorMessage(CartModelErrors.TooManyItems(cartModelRules.MaxItemPerCart));
        }
        
        public void SetOrderShippingPrice(ShippingPrice shipping, AccountRules accountRule)
        {
            ShippingPrice = shipping;
            if (shipping.IsValid && shipping.IsLocationActive)
                OrderPrices.TotalShippingPrice = shipping.FinalPrice;
            //OrderPrices.DefaultPrice += shipping.FinalPrice; 
            if (Account != null && Account.Roles != null)
            {
                var goldUserId = AccountRole.CustomerGold.Id;
                var silverUserId = AccountRole.CustomerSilver.Id;
                var bronzeUserId = AccountRole.CustomerBronze.Id;
                if (Account.Roles.Any(x => x.Id == goldUserId))
                {
                    var goldBenefit = accountRule.GoldRankBenefit;
                    SetShippingRankDiscountPercent(goldBenefit);
                }
                else if (Account.Roles.Any(x => x.Id == silverUserId))
                {
                    var silverBenefit = accountRule.SilverRankBenefit;
                    SetShippingRankDiscountPercent(silverBenefit);
                }
                else if (Account.Roles.Any(x => x.Id == bronzeUserId))
                {
                    var bronzeBenefit = accountRule.BronzeRankBenefit;
                    SetShippingRankDiscountPercent(bronzeBenefit);
                }
            }
            void SetShippingRankDiscountPercent(RankingBenefit rankingBenefit) 
            {
                if (OrderPrices.TotalShippingPrice <= 0)
                    return;
                var reducePercent = rankingBenefit.RankDiscountPercentOnShipping;
                var savedAmount = MoneyVndRoundUpRules.RoundAmountFromDecimal(ShippingPrice.DefaultPrice * ((decimal)reducePercent / 100m));
                ShippingPrice.UserRankReducedPrice = savedAmount;
                OrderPrices.TotalShippingPrice = ShippingPrice.FinalPrice;
            };
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
        public void SetUserRankDiscount(AccountRules accountRule, Account? userAccount)
        {
            if(userAccount!= null && userAccount.Roles != null)
            {
                var goldUserId = AccountRole.CustomerGold.Id;
                var silverUserId = AccountRole.CustomerSilver.Id;
                var bronzeUserId = AccountRole.CustomerBronze.Id;
                if (userAccount.Roles.Any(x => x.Id == goldUserId))
                {
                    var goldBenefit = accountRule.GoldRankBenefit;
                    SetUserRankDiscountPercent(goldBenefit.RankDiscountPercentOnOrder,goldBenefit.MaxAmountDiscountOnOrder);

                }
                else if (userAccount.Roles.Any(x => x.Id == silverUserId))
                {
                    var silverBenefit = accountRule.SilverRankBenefit;
                    SetUserRankDiscountPercent(silverBenefit.RankDiscountPercentOnOrder, silverBenefit.MaxAmountDiscountOnOrder);
                }
                else if (userAccount.Roles.Any(x => x.Id == bronzeUserId))
                {
                    var bronzeBenefit = accountRule.BronzeRankBenefit;
                    SetUserRankDiscountPercent(bronzeBenefit.RankDiscountPercentOnOrder, bronzeBenefit.MaxAmountDiscountOnOrder);
                }
            }
        }
        private void SetUserRankDiscountPercent(int reducePercent, decimal maxReduceAmount)
        {
            OrderPrices.UserRankDiscountPercent = reducePercent;
            var savedAmount = MoneyVndRoundUpRules.RoundAmountFromDecimal(OrderPrices.OrderPriceExcludeShipAndWarranty * ((decimal)reducePercent / 100m));
            var trueSavedAmount = Math.Clamp(savedAmount,0, maxReduceAmount);
            OrderPrices.UserRankDiscountAmount = trueSavedAmount;
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

using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IPromotionServices
    {
        Result ApplyPromotionOnCartModel(CartModel cartModel, Promotion promotion);
        void ApplyPromotionOnDiamond(Diamond diamond, List<Promotion> activePromotion);
        void ApplyPromotionOnJewerly(Jewelry jewelry, List<Promotion> activePromotion);

        void SetOrderPrice(CartModel cartModel);
    }
}

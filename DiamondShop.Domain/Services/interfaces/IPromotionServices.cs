using DiamondShop.Domain.Common.Carts;
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
        void SetOrderPrice(CartModel cartModel);
        Result ManualChangeStatus(Promotion promotion , Status status);
    }
}

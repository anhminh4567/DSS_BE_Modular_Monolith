using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.Promotions.Entities;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IDiscountService
    {
        Result ApplyDiscountOnCartModel(CartModel cartModel, Discount discount);
        void SetOrderPrice(CartModel cartModel);
    }
}

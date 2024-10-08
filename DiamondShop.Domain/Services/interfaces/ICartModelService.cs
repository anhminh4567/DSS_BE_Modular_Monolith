using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface ICartModelService
    {
        CartModel CreateFromUserCart(List<CartItem> cartItems);
        void InitOrderPrice(CartModel cartModel);
        void AssignProductAndItem(CartModel cartModel);
        void ValidateCartModel(CartModel cartModel);
        bool IsProduct(CartProduct item);
        Task<Result<CartModel>> Execute(List<CartItem> userCart);
    }
}

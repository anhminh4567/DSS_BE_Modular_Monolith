using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface ICartService
    {
        Task<List<CartItem>> AddProduct(AccountId accountId,CartItem cartProduct);
        Task<List<CartItem>> RemoveProduct(AccountId accountId, CartItemId cartProduct);
        Task<List<CartItem>> GetCartModel(AccountId accountId);
    }
}

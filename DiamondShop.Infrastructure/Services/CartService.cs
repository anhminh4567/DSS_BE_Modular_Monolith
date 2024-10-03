using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services
{
    internal class CartService : ICartService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CartService> _logger;
        private const string Cart_Key = "cart_userid";
        public CartService(IMemoryCache memoryCache, ILogger<CartService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<List<CartProduct>> AddProduct(AccountId accountId, CartProduct cartProduct)
        {
            var cartModel = GetCart(accountId);
            cartModel.Add(cartProduct);
            var key = Cart_Key.Replace("userid",accountId.Value);
            _memoryCache.Remove(key);
            return _memoryCache.Set(key, cartModel);
        }
        public async Task<List<CartProduct>> GetCartModel(AccountId accountId)
        {
            return GetCart(accountId) ;
        }

        public async Task<List<CartProduct>> RemoveProduct(AccountId accountId,CartProduct cartProduct)
        {
            var cartModel = GetCart(accountId);
            cartModel.Remove(cartProduct);
            var key = Cart_Key.Replace("userid", accountId.Value);
            _memoryCache.Remove(key);
            return _memoryCache.Set(key, cartModel);
        }
        private List<CartProduct> GetCart(AccountId accountId)
        {
            var key = Cart_Key.Replace("userid", accountId.Value);
            var tryget = _memoryCache.Get(key);
            if(tryget == null) 
            {
                return _memoryCache.Set(key, new List<CartProduct>());
            }
            return (List<CartProduct>)tryget;  
        }
    }
}

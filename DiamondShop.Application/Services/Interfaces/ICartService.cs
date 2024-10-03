using DiamondShop.Domain.Common;
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
        Task<List<CartProduct>> AddProduct(AccountId accountId,CartProduct cartProduct);
        Task<List<CartProduct>> RemoveProduct(AccountId accountId,CartProduct cartProduct);
        Task<List<CartProduct>> GetCartModel(AccountId accountId);
    }
}

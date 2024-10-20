using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
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
        //Task<CartModel> CreateFromUserCart(List<CartItem> cartItems);
        void InitOrderPrice(CartModel cartModel);
        void AssignProductAndItemCounter(CartModel cartModel);
        void SetCartModelValidation(CartModel cartModel);
        bool IsProduct(CartProduct item);
        void SetOrderPrice(CartModel cartModel);
        void SetShippingPrice(CartModel cartModel, ShippingPrice shippingPrice);
        Task<Result<CartModel>> Execute(List<CartProduct> products,List<Discount> givenDiscount, List<Promotion> givenPromotion , IDiamondPriceRepository _diamondPriceRepository, ISizeMetalRepository _sizeMetalRepository, IMetalRepository _metalRepository);
        Task<CartProduct?> FromCartItem(CartItem cartItem, IJewelryRepository _jewelryRepository, IJewelryModelRepository _jewelryModelRepository, IDiamondRepository _diamondRepository);
        Task AssignDefaultPriceToProduct(CartProduct cartProduct,IDiamondPriceRepository _diamondPriceRepository, ISizeMetalRepository _sizeMetalRepository, IMetalRepository _metalRepository);
        Task ValidateCartItems(CartModel cartModel);
    }
}

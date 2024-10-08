using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    public class CartModelService : ICartModelService
    {
        private readonly IPromotionServices _promotionServices;
        private readonly IDiscountService _discountService;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IDiscountRepository _discountRepository;

        public CartModelService(IPromotionServices promotionServices, IDiscountService discountService, IDiamondRepository diamondRepository, IJewelryRepository jewelryRepository, IJewelryModelRepository jewelryModelRepository, IPromotionRepository promotionRepository, IDiscountRepository discountRepository)
        {
            _promotionServices = promotionServices;
            _discountService = discountService;
            _diamondRepository = diamondRepository;
            _jewelryRepository = jewelryRepository;
            _jewelryModelRepository = jewelryModelRepository;
            _promotionRepository = promotionRepository;
            _discountRepository = discountRepository;
        }

        public void AssignProductAndItem(CartModel cartModel)
        {
            var products = cartModel.Products;
            cartModel.OrderCounter.TotalItem = products.Count;
            foreach (var product in products)
            {
                if (IsProduct(product))
                    cartModel.OrderCounter.TotalProduct += 1;
                else
                    cartModel.OrderCounter.TotalItem += 1;
            }
        }

        public CartModel CreateFromUserCart(List<CartItem> cartItems)
        {
            List<Task<CartProduct?>> tasks = new();
            cartItems.ForEach(cartItem => tasks.Add(FromCartItem(cartItem)));
            var cartProducts = Task.WhenAll(tasks).Result.ToList();
            ArgumentNullException.ThrowIfNull(cartProducts);
            var newCartModel = new CartModel { Products = cartProducts };
            return newCartModel;
        }

        public async Task<Result<CartModel>> Execute(List<CartItem> userCart)
        {
            ArgumentNullException.ThrowIfNull(userCart);
            var cartModel = CreateFromUserCart(userCart);
            AssignProductAndItem(cartModel);
            InitOrderPrice(cartModel);
            ValidateCartModel(cartModel);
            var getDiscount = await _discountRepository.GetActiveDiscount();
            var getPromotion = await _promotionRepository.GetActivePromotion();
            foreach (var discount in getDiscount ) 
            {
                var result = _discountService.ApplyDiscountOnCartModel(cartModel,discount);
                if(result.IsSuccess) 
                {
                    //do nothing
                }
            }
            foreach(var promotion in getPromotion)
            {
                var result = _promotionServices.ApplyPromotionOnCartModel(cartModel, promotion);
                if (result.IsSuccess)
                {
                    break;// only one promotion is applied at a time
                }
            }
            return Result.Ok(cartModel);
        }

        public void InitOrderPrice(CartModel cartModel)
        {
            cartModel.OrderPrices.DefaultPrice = cartModel.Products.Sum(product => product.ReviewPrice.DefaultPrice);
        }

        public bool IsProduct(CartProduct item)
        {
            if (item.Jewelry != null && item.Diamond != null)// when diamond is child of jewelry then it has 2 field
                return false;
            if (item.Jewelry != null || item.JewelryModel != null)
                return true;
            return true;// if diamond only have its own id, then it is a main product
        }

        public void ValidateCartModel(CartModel cartModel)
        {
            for (int i = 0; i < cartModel.Products.Count; i++)
            {
                var prod = cartModel.Products[i];
                if (prod.IsValid is false)
                {
                    if (IsProduct(prod))
                    {
                        cartModel.OrderCounter.TotalInvalidProduct += 1; // a product is also an item
                        cartModel.OrderCounter.TotalInvalidItem += 1;
                    }
                    else
                    {
                        cartModel.OrderCounter.TotalInvalidItem += 1;
                    }
                    cartModel.OrderValidation.InvalidItemIndex.Append(i);
                }
            }
        }
        private async Task<CartProduct?> FromCartItem(CartItem cartItem)
        {
            if (cartItem.JewelryId != null && cartItem.JewelryId != null && cartItem.JewelryModelId != null)
                throw new Exception("some how this cartItem have all 3 id, diamond, jewelry and model id, from cartItem id: " + cartItem.Id.Value);
            CartProduct cartProduct = new();
            if (cartItem.JewelryId is not null)
            {
                var jewelry = await _jewelryRepository.GetById(cartItem.JewelryId);
                cartProduct.Jewelry = jewelry;
            }
            if (cartItem.JewelryModelId is not null)
            {
                var jewelryModel = await _jewelryModelRepository.GetByIdMinimal(cartItem.JewelryModelId);
                cartProduct.JewelryModel = jewelryModel ;
            }
            if (cartItem.DiamondId is not null)
            {
                var diamond = await _diamondRepository.GetById(cartItem.DiamondId);
                cartProduct.Diamond = diamond ;
            }
            return cartProduct;
        }
    }
}

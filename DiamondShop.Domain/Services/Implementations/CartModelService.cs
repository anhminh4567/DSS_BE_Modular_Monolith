using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
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
        private readonly IDiamondServices _diamondServices;
        private readonly IJewelryService _jewelryService;
        private readonly IPromotionServices _promotionServices;
        private readonly IDiscountService _discountService;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IMainDiamondService _mainDiamondService;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private CartModel CurrentCart;
        public CartModelService(IDiamondServices diamondServices, IJewelryService jewelryService, IPromotionServices promotionServices, IDiscountService discountService, IDiamondRepository diamondRepository, IJewelryRepository jewelryRepository, IJewelryModelRepository jewelryModelRepository, ISizeMetalRepository sizeMetalRepository, IMainDiamondService mainDiamondService, IMainDiamondRepository mainDiamondRepository)
        {
            _diamondServices = diamondServices;
            _jewelryService = jewelryService;
            _promotionServices = promotionServices;
            _discountService = discountService;
            _diamondRepository = diamondRepository;
            _jewelryRepository = jewelryRepository;
            _jewelryModelRepository = jewelryModelRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _mainDiamondService = mainDiamondService;
            _mainDiamondRepository = mainDiamondRepository;
        }

        public void AssignProductAndItemCounter(CartModel cartModel)
        {
            var products = cartModel.Products;
            cartModel.OrderCounter.TotalItem = products.Count;
            foreach (var product in products)
            {
                if (IsProduct(product))
                {
                    cartModel.OrderCounter.TotalProduct += 1;
                    product.IsProduct = true;
                }
            }
        }


        public async Task<Result<CartModel>> Execute(List<CartProduct> products, List<Discount> givenDiscount, List<Promotion> givenPromotion, IDiamondPriceRepository _diamondPriceRepository, ISizeMetalRepository _sizeMetalRepository, IMetalRepository _metalRepository)
        {
            ArgumentNullException.ThrowIfNull(products);
            var cartModel = new CartModel { Products = products };
            CurrentCart = cartModel;
            if (CurrentCart.Products.Count == 0)
                return Result.Ok(CurrentCart);
            AssignProductAndItemCounter(CurrentCart);
            CurrentCart.Products.ForEach(async prd => await AssignDefaultPriceToProduct(prd, _diamondPriceRepository, _sizeMetalRepository, _metalRepository));
            ValidateCartItems(CurrentCart).Wait();
            SetCartModelValidation(CurrentCart);
            InitOrderPrice(CurrentCart);
            foreach (var discount in givenDiscount)
            {
                var result = _discountService.ApplyDiscountOnCartModel(CurrentCart, discount);
                if (result.IsSuccess)
                {
                    //do nothing
                }
            }
            foreach (var promotion in givenPromotion)
            {
                var result = _promotionServices.ApplyPromotionOnCartModel(CurrentCart, promotion);
                if (result.IsSuccess)
                {
                    break;// only one promotion is applied at a time
                }
            }
            SetOrderPrice(CurrentCart);
            return Result.Ok(CurrentCart);
        }

        public void InitOrderPrice(CartModel cartModel)
        {
            cartModel.OrderPrices.DefaultPrice = cartModel.Products.Where(p => p.IsValid).Sum(product => product.ReviewPrice.DefaultPrice);
            cartModel.OrderPrices.DefaultPrice += cartModel.ShippingPrice.FinalPrice;
        }

        public bool IsProduct(CartProduct item)
        {
            if (item.Jewelry != null && item.Diamond != null)// when diamond is child of jewelry then it has 2 field
                return false;
            if (item.Jewelry != null || item.JewelryModel != null)
                return true;
            return true;// if diamond only have its own id, then it is a main product
        }

        public void SetCartModelValidation(CartModel cartModel)
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
        public async Task<CartProduct?> FromCartItem(CartItem cartItem, IJewelryRepository _jewelryRepository, IJewelryModelRepository _jewelryModelRepository, IDiamondRepository _diamondRepository)
        {
            if (cartItem.JewelryId != null && cartItem.DiamondId != null && cartItem.JewelryModelId != null)
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
                cartProduct.JewelryModel = jewelryModel;
            }
            if (cartItem.DiamondId is not null)
            {
                var diamond = await _diamondRepository.GetById(cartItem.DiamondId);
                cartProduct.Diamond = diamond;
            }
            return cartProduct;
        }

        public async Task AssignDefaultPriceToProduct(CartProduct cartProduct, IDiamondPriceRepository _diamondPriceRepository, ISizeMetalRepository _sizeMetalRepository, IMetalRepository _metalRepository)
        {
            if (cartProduct.Jewelry != null && cartProduct.Diamond != null && cartProduct.JewelryModel != null)
                throw new Exception("some how this product have all 3 id, diamond, jewelry and model , from product: " + cartProduct.CartProductId);
            var reviewPrice = new CheckoutPrice();
            // if false do nothing, and assing price 0,  
            // price 0 is default for new CheckoutPrice();
            if (cartProduct.IsValid is false) { }
            else
            {
                if (cartProduct.Diamond is not null)
                {
                    var prices = _diamondPriceRepository.GetPriceByShapes(cartProduct.Diamond.DiamondShape).Result;
                    var diamondPrice = _diamondServices.GetDiamondPrice(cartProduct.Diamond, prices).Result;
                    if (diamondPrice == null)
                        reviewPrice.DefaultPrice = 0;
                    else
                    {
                        reviewPrice.DefaultPrice = diamondPrice.Price;
                    }


                }
                else if (cartProduct.Jewelry is not null)
                {
                    //TODO: assign default price to jewelry
                    _jewelryService.AddPrice(cartProduct.Jewelry, _sizeMetalRepository);
                    reviewPrice.DefaultPrice = cartProduct.Jewelry.Price;
                }
                else if (cartProduct.JewelryModel is not null)
                {
                    //TODO: assign default price to jewelry model
                }
            }
            cartProduct.ReviewPrice = reviewPrice;
        }

        public Task ValidateCartItems(CartModel cartModel)
        {
            var products = cartModel.Products;
            foreach (var product in products)
            {
                if (product.Jewelry != null && product.Diamond != null && product.JewelryModel != null)
                    throw new Exception("some how this product have all 3 id, diamond, jewelry and model , from product: " + product.CartProductId);
                var result = CheckIsValid(product);
                if (result.IsFailed)
                {
                    product.IsAvailable = false;
                    product.IsValid = false;
                }
            }
            int[] unavailableItemIndex = products.Where(p => p.IsAvailable is false).Select(p => products.IndexOf(p)).ToArray();
            cartModel.OrderValidation.UnavailableItemIndex = unavailableItemIndex;
            //get all the item in cart that is of product type, not item
            List<CartProduct> getParentProduct = products.Where(p => p.IsProduct).ToList();
            foreach (var parentProduct in getParentProduct)
            {
                // only diamond have parent, jewelry and jewelry model dont have parent
                var getDiamondChildItem = products.Where(p => p.Diamond != null
                    && (p.Jewelry != null || p.JewelryModel != null)
                    && p.IsProduct == false)
                    .ToList();
                if (getDiamondChildItem.Any(d => d.IsValid == false) || parentProduct.IsValid == false)
                {
                    //if any of the diamond child item is invalid, then all is invalid, also the parent
                    getDiamondChildItem.ForEach(d => d.IsValid = false);
                    parentProduct.IsValid = false;
                }
            }
            return Task.CompletedTask;
        }
        private Result CheckIsValid(CartProduct product)
        {
            if (product.Diamond is not null)
            {
                if (product.Diamond.IsActive is false)
                    return Result.Fail("not active");
                if (product.Diamond.DiamondPrice!.ForUnknownPrice != null)
                    return Result.Fail("unknown price");
                if (product.Diamond.IsSold is true)
                    Result.Fail("already sold");
                if(product.Jewelry != null || product.JewelryModel != null)
                    return CheckIfDiamondJewelryIsValid(product);
                
                return CheckDuplicate(CurrentCart,product);
            }
            if (product.Jewelry is not null)
                return product.Jewelry.IsSold ? Result.Fail("already sold") : CheckDuplicate(CurrentCart, product);
            if (product.JewelryModel is not null) { }
            return Result.Fail("unknonw product type");
            //return product.JewelryModel.
        }
        private Result CheckDuplicate(CartModel cartModel , CartProduct cartProduct) 
        {
            var products = cartModel.Products;
            List<CartProduct> matchedProduct = new();
            if(cartProduct.Diamond != null)
            {
                matchedProduct = products.Where(p => p.Diamond.Id == cartProduct.Diamond.Id).ToList(); 
            }
            else if(cartProduct.Jewelry != null)
            {
                matchedProduct = products.Where(p => p.Jewelry.Id == cartProduct.Jewelry.Id).ToList();
            }
            matchedProduct.ForEach(p => p.IsDuplicate = true);
            if(matchedProduct.Count > 0)
            {
                return Result.Fail("duplicate products found");
            }
            else
            {
                return Result.Ok();
            }
        }
        private Result CheckIfDiamondJewelryIsValid(CartProduct diamondJewelry)
        {
            if (diamondJewelry.Jewelry != null)
            {
                var attachedJewelryId = diamondJewelry.Diamond!.JewelryId;
                if (attachedJewelryId != null)// means the diamond is attached to a jewelry already, check if the attached 
                                              // jewelry is correct one with the one in CartProduct
                {
                    if (diamondJewelry.Jewelry.Id != attachedJewelryId)
                        return Result.Fail("attached jewelry id is not the same as the jewelry id in the cart product");
                    else
                        return Result.Ok();
                }
                return _mainDiamondService.CheckMatchingDiamond(diamondJewelry.Jewelry.ModelId,new List<Diamond> { diamondJewelry.Diamond },_mainDiamondRepository).Result;
            }
            else // else check with JewelryModels
            {
                return _mainDiamondService.CheckMatchingDiamond(diamondJewelry.JewelryModel.Id, new List<Diamond> { diamondJewelry.Diamond }, _mainDiamondRepository).Result;
            }
        }
        public void SetOrderPrice(CartModel cartModel)
        {
            var orderPrice = cartModel.OrderPrices;
            foreach (var product in cartModel.Products)
            {
                if (product.IsValid)
                {
                    orderPrice.DiscountAmountSaved += product.ReviewPrice.DiscountAmountSaved;
                    orderPrice.PromotionAmountSaved += product.ReviewPrice.PromotionAmountSaved;
                }
            }
        }

        public void SetShippingPrice(CartModel cartModel, ShippingPrice shippingPrice)
        {
            if(shippingPrice != null)
                cartModel.ShippingPrice = shippingPrice;
        }
    }

}

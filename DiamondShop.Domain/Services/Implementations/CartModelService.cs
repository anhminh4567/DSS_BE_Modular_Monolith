using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Common.Carts.ErrorMessages;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Warranties;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IWarrantyRepository _warrantyRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPromotionRepository _promotionRepository; 
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private CartModel CurrentCart;

        public CartModelService(IDiamondServices diamondServices, IJewelryService jewelryService, IPromotionServices promotionServices, IDiscountService discountService, IDiamondRepository diamondRepository, IJewelryRepository jewelryRepository, IJewelryModelRepository jewelryModelRepository, ISizeMetalRepository sizeMetalRepository, IMainDiamondService mainDiamondService, IMainDiamondRepository mainDiamondRepository, IDiamondPriceRepository diamondPriceRepository, IWarrantyRepository warrantyRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IOrderRepository orderRepository, IPromotionRepository promotionRepository)
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
            _diamondPriceRepository = diamondPriceRepository;
            _warrantyRepository = warrantyRepository;
            _optionsMonitor = optionsMonitor;
            _orderRepository = orderRepository;
            _promotionRepository = promotionRepository;
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


        public async Task<Result<CartModel>> ExecuteNormalOrder(List<CartProduct> products, List<Discount> givenDiscount, List<Promotion> givenPromotion, ShippingPrice shipPrice, Account? account, CartModelRules cartModelRules)
        {
            ArgumentNullException.ThrowIfNull(products);
            var promotionRule = _optionsMonitor.CurrentValue.PromotionRule;
            var cartModel = new CartModel { Products = products , Account = account };
            CurrentCart = cartModel;
            if (CurrentCart.Products.Count == 0)
                return Result.Ok(CurrentCart);
            AssignProductAndItemCounter(CurrentCart);
            CurrentCart.Products.ForEach(async prd => await AssignDefaultPriceToProduct(prd));
            ValidateCartItems(CurrentCart).Wait();
            SetCartModelValidation(CurrentCart);
            InitOrderPrice(CurrentCart);
            if(CurrentCart.Account != null)
            {
                if (CurrentCart.Account.CustomerOrders == null)
                    CurrentCart.Account.CustomerOrders = await _orderRepository.GetUserOrders(CurrentCart.Account);
            }
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
                var result = _promotionServices.ApplyPromotionOnCartModel(CurrentCart, promotion, promotionRule);
                if (result.IsSuccess)
                {
                    break;// only one promotion is applied at a time
                }
            }
            CurrentCart.SetUserRankDiscount(_optionsMonitor.CurrentValue.AccountRules,account);
            CurrentCart.SetOrderShippingPrice(shipPrice,_optionsMonitor.CurrentValue.AccountRules);
            CurrentCart.SetWarrantyTotalPrice();
            CurrentCart.SetErrorMessages();
            CurrentCart.ValidateCartRules(cartModelRules);
            //CurrentCart.SetOrderPrice();
            
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
                cartProduct.EngravedFont = cartItem.EngravedFont;
                cartProduct.EngravedText = cartItem.EngravedText;
                //return here, since the jewelry wont contain id for diamond or jewelry mode, 
                // but jewelry model might have diamond attached to it, so diamond might have both jewelry and jewelry model id
                return cartProduct;
            }
            if (cartItem.JewelryModelId is not null)
            {
                var jewelryModel = await _jewelryModelRepository.GetByIdMinimal(cartItem.JewelryModelId);
                cartProduct.JewelryModel = jewelryModel;
                cartProduct.EngravedFont = cartItem.EngravedFont;
                cartProduct.EngravedText = cartItem.EngravedText;
            }
            if (cartItem.DiamondId is not null)
            {
                var diamond = await _diamondRepository.GetById(cartItem.DiamondId);
                cartProduct.Diamond = diamond;
            }
            return cartProduct;
        }

        public async Task AssignDefaultPriceToProduct(CartProduct cartProduct)
        {
            if (cartProduct.Jewelry != null && cartProduct.Diamond != null && cartProduct.JewelryModel != null)
                throw new Exception("some how this product have all 3 id, diamond, jewelry and model , from product: " + cartProduct.CartProductId);
            var reviewPrice = new CheckoutPrice();
            // if false do nothing, and assing price 0,  
            // price 0 is default for new CheckoutPrice();
            if (cartProduct.Diamond is not null)
            {
                bool isFancyShape = DiamondShape.IsFancyShape(cartProduct.Diamond.DiamondShapeId);
                List<DiamondPrice> prices = new();
                prices = _diamondPriceRepository.GetPrice(cartProduct.Diamond.Cut.Value,cartProduct.Diamond.DiamondShape,cartProduct.Diamond.IsLabDiamond).Result;
                var diamondPrice = _diamondServices.GetDiamondPrice(cartProduct.Diamond, prices).Result;
                reviewPrice.DefaultPrice = cartProduct.Diamond.TruePrice;
            }
            else if (cartProduct.Jewelry is not null)
            {
                _jewelryService.AddPrice(cartProduct.Jewelry, _sizeMetalRepository);
                //foreach (var diamond in cartProduct.Jewelry.Diamonds)
                //{
                //    var prices = _diamondPriceRepository.GetPriceByShapes(diamond.DiamondShape,diamond.IsLabDiamond).Result;
                //    var diamondPrice = _diamondServices.GetDiamondPrice(diamond, prices).Result;
                //    diamond.DiamondPrice = diamondPrice;
                //}
                //cartProduct.Jewelry.D_Price = cartProduct.Jewelry.Diamonds.Sum(d => d.TruePrice);
                reviewPrice.DefaultPrice = cartProduct.Jewelry.TotalPrice;
            }
            else if (cartProduct.JewelryModel is not null)
            {
                //TODO: assign default price to jewelry model
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
                    product.ErrorMessage = result.Errors.FirstOrDefault()?.Message;
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
            var productIndex = CurrentCart.Products.IndexOf(product);
            if (product.Diamond is not null)
            {
                if (product.Diamond.Status == ProductStatus.Sold)
                    return Result.Fail(CartModelErrors.CartProductError.Sold(product));
                if (product.Diamond.Status != ProductStatus.Active && product.Diamond.Status != ProductStatus.LockForUser)
                    return Result.Fail(CartModelErrors.CartProductError.IncorrectStateForSale(product));
                if (product.Diamond.Status == ProductStatus.LockForUser) 
                {
                    if (product.Diamond.ProductLock is null || CurrentCart.Account == null)
                        return Result.Fail("");
                    if (product.Diamond.ProductLock.AccountId != null &&  (product.Diamond.ProductLock.AccountId != CurrentCart.Account.Id))
                        return Result.Fail(CartModelErrors.CartProductError.NotLockForThisUser(product,CurrentCart.Account));
                }
                if (product.Diamond.DiamondPrice!.ForUnknownPrice != null)
                    return Result.Fail(CartModelErrors.CartProductError.UnknownPrice(productIndex + 1, product));
                if (product.Diamond.IsSetForJewelry)
                    return Result.Fail(DiamondErrors.DiamondAssignedToJewelryAlready());
                if (product.Jewelry != null || product.JewelryModel != null)
                {
                    var result = CheckIfDiamondJewelryIsValid(product);
                    if (result.IsSuccess)
                        return CheckDuplicate(CurrentCart, product);
                    return result;
                }
                return CheckDuplicate(CurrentCart, product);
            }
            if (product.Jewelry is not null)
            {
                if (product.Jewelry.Status == ProductStatus.Sold)
                    return Result.Fail(CartModelErrors.CartProductError.Sold(product));
                if (product.Jewelry.IsAllDiamondPriceKnown == false)
                    return Result.Fail(CartModelErrors.CartProductError.UnknownPrice(productIndex,product));
                if (product.Jewelry.Status != ProductStatus.Active 
                    && product.Jewelry.Status != ProductStatus.LockForUser
                    && product.Jewelry.Status != ProductStatus.PreOrder)
                    return Result.Fail(CartModelErrors.CartProductError.IncorrectStateForSale(product));
                if (product.Jewelry.Status == ProductStatus.LockForUser)
                {
                    if (product.Jewelry.ProductLock is null || CurrentCart.Account == null)
                        return Result.Fail("Sản phẩm khóa cho khách khác");
                    if (product.Jewelry.ProductLock.AccountId != CurrentCart.Account.Id)
                        return Result.Fail(CartModelErrors.CartProductError.NotLockForThisUser(product, CurrentCart.Account));
                }
                return CheckDuplicate(CurrentCart, product);
            }
            if (product.JewelryModel is not null) { }
            return Result.Fail(CartModelErrors.CartProductError.UnknownProductType);
            //return product.JewelryModel.
        }
        private Result CheckDuplicate(CartModel cartModel, CartProduct cartProduct)
        {
            var products = cartModel.Products;
            List<CartProduct> matchedProduct = new();
            //products.Remove(cartProduct);
            int productIndex = products.IndexOf(cartProduct);
            if (cartProduct.Diamond != null)
            {
                matchedProduct = products.Where(p => p.Diamond != null
                && products.IndexOf(p) != productIndex
                && p.Diamond.Id == cartProduct.Diamond.Id).ToList();
            }
            else if (cartProduct.Jewelry != null)
            {
                matchedProduct = products.Where(p => p.Jewelry != null
                && p.Diamond == null
                && products.IndexOf(p) != productIndex
                && p.Jewelry.Id == cartProduct.Jewelry.Id).ToList();
            }
            //products.Add(cartProduct);
            matchedProduct.ForEach(p => p.IsDuplicate = true);


            if (matchedProduct.Count > 0)
            {
                cartProduct.IsDuplicate = true;
                return Result.Fail(CartModelErrors.CartProductError.Duplicate);
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
                        return Result.Fail(CartModelErrors.CartProductError.JewelryType.DiamondNotCorrect);
                    else
                        return Result.Ok();
                }
                return _mainDiamondService.CheckMatchingDiamond(diamondJewelry.Jewelry.ModelId, new List<Diamond> { diamondJewelry.Diamond }).Result;
            }
            else // else check with JewelryModels
            {
                return _mainDiamondService.CheckMatchingDiamond(diamondJewelry.JewelryModel.Id, new List<Diamond> { diamondJewelry.Diamond }).Result;
            }
        }
        //TODO: add warranty to default price ==> it follows discount and promotion flow
        public async Task<List<CartProduct>> GetCartProduct(List<CartItem> items)
        {
            List<CartProduct> cartProducts = new();
            List<Warranty> GetAllWarranties = await _warrantyRepository.GetAll();
            foreach (var item in items)
            {
                var cartProduct = FromCartItem(item, _jewelryRepository, _jewelryModelRepository, _diamondRepository).Result;
                if (cartProduct != null)
                {
                    cartProducts.Add(cartProduct);
                    Warranty usedWarranty;
                    usedWarranty = GetAllWarranties.FirstOrDefault(w => w.Type == item.WarrantyType && w.Code == item.WarrantyCode);
                    if(usedWarranty == null)
                    {
                        //if not found by code, get the warrany default price = 0
                        var type =( item.JewelryId != null || item.JewelryModelId != null ) && item.DiamondId == null
                            ? WarrantyType.Jewelry 
                            : WarrantyType.Diamond;
                        usedWarranty = GetAllWarranties.FirstOrDefault(w => w.Type == type && w.Price == 0);
                    }
                    cartProduct.CurrentWarrantyApplied = usedWarranty;
                    cartProduct.CurrentWarrantyPrice = MoneyVndRoundUpRules.RoundAmountFromDecimal( usedWarranty.Price);
                    //this is not importrant, can be ignored, warranty should be place seperately like above is enought
                    cartProduct.ReviewPrice.WarrantyPrice = cartProduct.CurrentWarrantyPrice;
                }
            }
            return cartProducts;
        }

        public Task<Result<CartModel>> ExecuteCustomOrder(List<CartProduct> products, List<Discount> givenDiscount, List<Promotion> givenPromotion, ShippingPrice shipPrice, Account? account)
        {
            throw new NotImplementedException();
        }
    }

}

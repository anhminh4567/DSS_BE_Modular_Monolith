using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Carts
{
    public class CartModelDto
    {
        public CartModelPromotionDto Promotion { get; set; }
        public List<DiscountDto> DiscountsApplied { get; set; }
        public CartModelPriceDto OrderPrices { get; set; }
        public ShippingPriceDto ShippingPrice { get; set; }
        public CartModelCounterDto OrderCounter { get; set; }
        public CartModelValidationDto OrderValidation { get; set; }
        public List<CartProductDto> Products { get; set; }
        public decimal? PayAmount { get; set; }
        public decimal? DepositAmount { get; set; }
    }
}

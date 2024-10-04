using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;

namespace DiamondShop.Domain.Common.Carts
{
    public class CartModel
    {
        public CartModelPromotion Promotion { get; set; } = new CartModelPromotion() ;
        public CheckoutPrice OrderPrices { get; set; } = new();
        public ShippingPrice? ShippingPrice { get; set; }
        public CartModelCounter? OrderCounter { get; set; }
        public CartModelValidation? OrderValidation { get; set; }
        public List<CartProduct> Products { get; set; } = new();
    }

}

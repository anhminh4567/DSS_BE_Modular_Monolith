using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;

namespace DiamondShop.Domain.Common.Carts
{
    public class CartModelDiscountInfo
    {
        public Promotion? Promotion { get; set; }
        public Discount? Discount { get; set; }
        public decimal TotalPromotionPrice { get; set; }
        public decimal TotalDiscountPrice { get; set; }
    }

}

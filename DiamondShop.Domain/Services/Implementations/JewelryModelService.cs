using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;

namespace DiamondShop.Domain.Services.Implementations
{
    public class JewelryModelService : IJewelryModelService
    {
        public Task<Discount?> AssignJewelryModelDiscount(JewelryModel model, List<Discount> discounts)
        {
            return AssignJewelryModelDiscountGlobal(model, discounts);
        }

        public static async Task<Discount?> AssignJewelryModelDiscountGlobal(JewelryModel model, List<Discount> discounts)
        {
            Discount mostValuableDiscont = null;
            foreach (var discount in discounts)
            {
                var requirements = discount.DiscountReq;
                foreach (var req in requirements)
                {
                    if (req.TargetType != TargetType.Jewelry_Model)
                        continue;
                    if (req.ModelId != model.Id)
                        continue;
                    if (mostValuableDiscont == null)
                    {
                        mostValuableDiscont = discount;
                        break;
                    }
                    else
                    {
                        if (discount.DiscountPercent >= mostValuableDiscont.DiscountPercent)
                        {
                            mostValuableDiscont = discount;
                            break;
                        }
                    }
                }
            }
            return mostValuableDiscont;
        }
    }
}

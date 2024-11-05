using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Repositories;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IJewelryModelService
    {
        Task<Discount?> AssignJewelryModelDiscount(JewelryModel jewelry, List<Discount> discounts);
    }
}

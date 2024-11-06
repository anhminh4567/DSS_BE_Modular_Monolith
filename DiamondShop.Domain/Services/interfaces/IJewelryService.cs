using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IJewelryService
    {
        string GetSerialCode(JewelryModel model, Metal metal, Size size);
        bool SetupUnmapped(List<Jewelry> jewelries, SizeMetal sizeMetal);
        bool SetupUnmapped(List<Jewelry> jewelries, List<SizeMetal> sizeMetals);
        Task<Discount?> AssignJewelryDiscount(Jewelry jewelry, List<Discount> discounts);
        Jewelry AddPrice(Jewelry jewelry, SizeMetal sizeMetal);
        Jewelry AddPrice(Jewelry jewelry, ISizeMetalRepository sizeMetalRepository);
        IQueryable<Jewelry> GetJewelryQueryFromModel(JewelryModelId modelId, MetalId metalId, SizeId sizeId, SideDiamondOpt? sideDiamondOpt);
    }
}

using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IJewelryService
    {
        void AddPrice(List<Jewelry> jewelries, List<SizeMetal> sizeMetals);
        Jewelry AddPrice(Jewelry jewelry, SizeMetal sizeMetal);
        Jewelry AddPrice(Jewelry jewelry, ISizeMetalRepository sizeMetalRepository);
    }
}

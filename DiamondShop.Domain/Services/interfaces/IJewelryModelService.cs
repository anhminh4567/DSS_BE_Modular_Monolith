using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Repositories;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IJewelryModelService
    {
        void GetSizeMetalPrice(List<SizeMetal> sizeMetals);
        void GetSizeMetalPrice(SizeMetal sizeMetal);
    }
}

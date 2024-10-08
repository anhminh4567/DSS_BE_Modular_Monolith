using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Domain.Repositories.JewelryModelRepo
{
    public interface IMainDiamondRepository : IBaseRepository<MainDiamondReq>
    {
        public Task CreateRange(List<MainDiamondShape> shapes, CancellationToken token = default);
        public Task<List<MainDiamondReq>> GetCriteria(JewelryModelId modelId);

    }
}

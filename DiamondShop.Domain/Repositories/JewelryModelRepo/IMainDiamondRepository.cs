using DiamondShop.Domain.Models.JewelryModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.JewelryModelRepo
{
    public interface IMainDiamondRepository : IBaseRepository<MainDiamondReq>
    {
        public Task CreateShapes(List<MainDiamondShape> shapes, CancellationToken token = default);
        public List<MainDiamondReq> GetMainDiamondShapes(string modelId);
    }
}

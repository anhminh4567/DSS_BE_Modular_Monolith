using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.JewelryModelRepo
{
    public interface ISizeMetalRepository : IBaseRepository<SizeMetal>
    {
        public Task<SizeMetal?> GetModelSizeMetal(JewelryModelId modelId, SizeId sizeId, MetalId metalId);
        public Task CreateRange(List<SizeMetal> sizeMetalList, CancellationToken token);
    }
}

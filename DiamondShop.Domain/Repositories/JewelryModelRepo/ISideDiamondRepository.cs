using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.JewelryModelRepo
{
    public interface ISideDiamondRepository : IBaseRepository<SideDiamondOpt> 
    {
        public Task<List<SideDiamondOpt>> GetByModelId(JewelryModelId modelId);
        public Task CreateRange(List<SideDiamondOpt> sideDiamondOpts, CancellationToken token = default);
        public Task<List<SideDiamondOpt>?> GetSideDiamondOption(List<SideDiamondOptId> sideDiamondOptId);
    }
}

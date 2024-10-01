using DiamondShop.Domain.Models.JewelryModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.JewelryModelRepo
{
    public interface ISideDiamondRepository : IBaseRepository<SideDiamondReq> 
    {
        public Task CreateOpts(List<SideDiamondOpt> sideDiamondOpts, CancellationToken token = default);
    }
}

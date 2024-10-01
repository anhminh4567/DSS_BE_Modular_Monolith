using DiamondShop.Domain.Models.JewelryModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.JewelryModelRepo
{
    public interface ISizeMetalRepository : IBaseRepository<SizeMetal>
    {
        public Task CreateRange(List<SizeMetal> sizeMetalList, CancellationToken token);
    }
}

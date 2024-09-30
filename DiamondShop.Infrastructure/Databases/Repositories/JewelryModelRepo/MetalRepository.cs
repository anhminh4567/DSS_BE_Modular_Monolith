using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo
{
    internal class MetalRepository : BaseRepository<Metal>, IMetalRepository
    {
        public MetalRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Metal?> GetById(params object[] ids)
        {
            MetalId id = (MetalId)ids[0];
            return await _set.FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}

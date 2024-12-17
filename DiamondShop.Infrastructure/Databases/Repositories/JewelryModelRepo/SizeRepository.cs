using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.JewelryModelRepo
{
    internal class SizeRepository : BaseRepository<Size>, ISizeRepository
    {
        public SizeRepository(DiamondShopDbContext dbContext): base(dbContext) { }
        public async Task<Size?> GetByValue(float value, string unit = null)
        {
            unit = unit is null ? Size.Milimeter : unit;
            return await _dbContext.Sizes.FirstOrDefaultAsync(p => p.Value == value && p.Unit == unit);
        }

    }
}

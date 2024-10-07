using DiamondShop.Domain.Models.JewelryModels;
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
    internal class JewelryModelCategoryRepository : BaseRepository<JewelryModelCategory>, IJewelryModelCategoryRepository
    {
        public JewelryModelCategoryRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<bool> CheckDuplicate(string name)
        {
            return await _set.AnyAsync(p => p.Name == name);
        }
        
        public override async Task<JewelryModelCategory?> GetById(params object[] ids)
        {
            JewelryModelCategoryId id = (JewelryModelCategoryId)ids[0];
            return await _set.Include(p => p.ParentCategory).FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}

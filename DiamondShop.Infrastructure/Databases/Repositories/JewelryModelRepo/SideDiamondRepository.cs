using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
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
    internal class SideDiamondRepository : BaseRepository<SideDiamondOpt>, ISideDiamondRepository
    {
        public SideDiamondRepository(DiamondShopDbContext dbContext) : base(dbContext) { }

        public async Task CreateRange(List<SideDiamondOpt> sideDiamondOpts, CancellationToken token = default)
        {
            await _set.AddRangeAsync(sideDiamondOpts, token);
        }
        public async Task<List<SideDiamondOpt>> GetByModelId(JewelryModelId modelId)
        {
            return await _set.Include(p => p.Shape).Where(p => p.ModelId == modelId).ToListAsync();
        }
        public override async Task<SideDiamondOpt?> GetById(params object[] ids)
        {
            var id = (SideDiamondOptId)ids[0];
            return await _set.Include(p => p.Shape).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<List<SideDiamondOpt>?> GetSideDiamondOption(List<SideDiamondOptId> sideDiamondOptId)
        {
            return _set.Where(p => sideDiamondOptId.Contains(p.Id)).ToList();
        }

        public Task<bool> CheckExist(JewelryModelId modelId, DiamondShapeId diamondShapeId, float caratWeight, int quantity, Clarity clarityMin, Clarity clarityMax, Color colorMin, Color colorMax, SettingType settingType, bool isLabGrown)
        {
            return _set.AnyAsync(p => p.ModelId == modelId &&
            p.ShapeId == diamondShapeId && p.CaratWeight == caratWeight && p.Quantity == quantity &&
            p.ClarityMin == clarityMin && p.ClarityMax == clarityMax && p.ColorMin == colorMin && p.ColorMax == colorMax &&
            p.SettingType == settingType && p.IsLabGrown == isLabGrown);
        }
    }
}

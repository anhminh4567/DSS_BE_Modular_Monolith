using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
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
        public Task<bool> CheckExist(JewelryModelId modelId,DiamondShapeId diamondShapeId, float caratWeight, int quantity, Clarity clarityMin, Clarity clarityMax, Color colorMin, Color colorMax,SettingType settingType, bool isLabGrown);
    }
}

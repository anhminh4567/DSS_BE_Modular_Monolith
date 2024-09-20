using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public class MainDiamond : Entity<MainDiamondId>
    {
        public JewelryModelId JewelryModelId { get; set; }
        public List<MainDiamondShape> DiamondShapes { get; set; } = new ();
        public SettingType SettingType { get; set; }
        public int Quantity { get; set; }
    }
}

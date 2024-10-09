using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Jewelries.Entities
{
    public class JewelrySideDiamond : Entity<JewelrySideDiamondId>
    {
        public JewelryId JewelryId { get; set; }
        public float Carat { get; set; }
        public int Quantity { get; set; }
        public Color ColorMin { get; set; }
        public Color ColorMax { get; set; }
        public Clarity ClarityMin { get; set; }
        public Clarity ClarityMax { get; set; }
        public SettingType SettingType { get; set; }
        public JewelrySideDiamond() { }
        public static JewelrySideDiamond Create(JewelryId jewelryId, SideDiamondOpt sideDiamondOpt)
        {
            return new JewelrySideDiamond()
            {
                Id = JewelrySideDiamondId.Create(),
                JewelryId = jewelryId,
                Carat = sideDiamondOpt.CaratWeight,
                Quantity = sideDiamondOpt.Quantity,
                ColorMin = sideDiamondOpt.SideDiamondReq.ColorMin,
                ColorMax = sideDiamondOpt.SideDiamondReq.ColorMax,
                ClarityMin = sideDiamondOpt.SideDiamondReq.ClarityMin,
                ClarityMax = sideDiamondOpt.SideDiamondReq.ClarityMax,
                SettingType = sideDiamondOpt.SideDiamondReq.SettingType
            };
        }
        public static JewelrySideDiamond Create(JewelryId jewelryId, float carat, int quantity, Color colorMin, Color colorMax, Clarity clarityMin, Clarity clarityMax, SettingType settingType, JewelrySideDiamondId givenId = null)
        {
            return new JewelrySideDiamond()
            {
                Id = givenId is null ? JewelrySideDiamondId.Create() : givenId,
                Carat = carat,
                Quantity = quantity,
                ColorMin = colorMin,
                ColorMax = colorMax,
                ClarityMin = clarityMin,
                ClarityMax = clarityMax,
                SettingType = settingType,
            };
        }
    }
}

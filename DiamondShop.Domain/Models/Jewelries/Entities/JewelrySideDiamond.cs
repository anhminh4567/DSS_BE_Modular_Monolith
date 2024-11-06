using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Jewelries.Entities
{
    public class JewelrySideDiamond
    {
        public DiamondShapeId? DiamondShapeId { get; set; }   
        public DiamondShape? DiamondShape { get; set; }
        public float Carat { get; set; }
        public int Quantity { get; set; }
        public bool IsLabGrown { get; set; } = true;
        //public Cut? AverageCut { get; set; }
        public Color ColorMin { get; set; }
        public Color ColorMax { get; set; }
        public Clarity ClarityMin { get; set; }
        public Clarity ClarityMax { get; set; }
        public SettingType SettingType { get; set; }
        [NotMapped]
        public bool IsFancyShape { get => DiamondShape.IsFancyShape(DiamondShapeId); }
        [NotMapped]
        public List<DiamondPrice> DiamondPrice { get; set; } = new();
        [NotMapped]
        public int TotalPriceMatched { get => DiamondPrice.Count; }
        //[NotMapped]
        //public DiamondPrice DiamondPriceFound { get; set; }
        [NotMapped]
        public decimal AveragePricePerCarat { get; set; } = 0;
        [NotMapped]
        public decimal TotalPrice { get => AveragePricePerCarat * Quantity; }
        [NotMapped]
        public bool IsPriceKnown { get => AveragePricePerCarat > 0; }
        // price is not just from diamondPrice, must * the amount of diamond to get real price
        // price from diamondPrice is average price per diamond not total price
        [NotMapped]
        public float AverageCarat { get => (float)Carat / (float)Quantity; }
        private JewelrySideDiamond() { }
        public static JewelrySideDiamond Create(SideDiamondOpt sideDiamondOpt)
        {
            return new JewelrySideDiamond()
            {
                Carat = sideDiamondOpt.CaratWeight,
                Quantity = sideDiamondOpt.Quantity,
                ColorMin = sideDiamondOpt.ColorMin,
                ColorMax = sideDiamondOpt.ColorMax,
                ClarityMin = sideDiamondOpt.ClarityMin,
                ClarityMax = sideDiamondOpt.ClarityMax,
                SettingType = sideDiamondOpt.SettingType,
                DiamondShapeId = sideDiamondOpt.ShapeId,
                IsLabGrown = sideDiamondOpt.IsLabGrown,
            };
        }
        public static JewelrySideDiamond Create(JewelryId jewelryId, float carat, int quantity, Color colorMin, Color colorMax, Clarity clarityMin, Clarity clarityMax, SettingType settingType)
        {
            return new JewelrySideDiamond()
            {
                Carat = carat,
                Quantity = quantity,
                ColorMin = colorMin,
                ColorMax = colorMax,
                ClarityMin = clarityMin,
                ClarityMax = clarityMax,
                SettingType = settingType,
            };
        }
        public void SetCorrectPrice()
        {
            var caratCorrectPrice = AveragePricePerCarat * (decimal)Quantity;
            var price = MoneyVndRoundUpRules.RoundAmountFromDecimal(caratCorrectPrice);
            //TotalPrice = price;
            //AveragePricePerCarat = price;
        }
    }
}

using DiamondShop.Domain.Models.Diamonds.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class DiamondPriceRules
    {
        public static DiamondPriceRules Default = new DiamondPriceRules();
        public static string Type = typeof(DiamondPriceRules).AssemblyQualifiedName;
        public static string key = "DiamondPriceRulesVer1";
        public decimal MinPriceOffsetPercent { get; set; } = 100 - 90;//%
        public decimal MaxPriceOffsetPercent { get; set; } = 100 + 90;//%
        public string DefaultRoundCriteriaPriceBoard { get; set; } = "giá theo tiêu chí 3X";
        public string DefaultFancyCriteriaPriceBoard { get; set; } = "giá theo tiêu chí 2X";



    }
}

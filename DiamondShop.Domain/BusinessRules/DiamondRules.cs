using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public  class DiamondRules
    {
        public static decimal MinPriceOffset { get; set; } = 0.1M;
        public static decimal MaxPriceOffset{ get; set; } = 10M;
        public static decimal BiggestSideDiamondCarat { get; set; } = 0.2M;
    }
    public class DiamondRule
    {
        public static DiamondRule Default = new DiamondRule();
        public static string Type = typeof(DiamondRule).AssemblyQualifiedName;
        public static string key = "DiamondRule";
        public decimal MinPriceOffsetPercent { get; set; } = 10;
        public decimal MaxPriceOffsetPercent { get; set; } = 190;
        public decimal BiggestSideDiamondCarat { get; set; } = 0.2M;
    }
}

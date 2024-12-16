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
        public static string key = "DiamondPriceRulesVer2";
        public decimal MinPriceOffsetPercent { get; set; } = 100 - 90;//%
        public decimal MaxPriceOffsetPercent { get; set; } = 100 + 90;//%
        public string DefaultRoundCriteriaPriceBoard { get; set; } = "giá theo tiêu chí 3X(X cut, X polish, X symmetry)";
        public string DefaultFancyCriteriaPriceBoard { get; set; } = "giá theo tiêu chí 2X(X polish, X symmetry)";

        public string DefaultSideDiamondCriteriaPriceBoard { get; set; } = "Giá chung cho mọi shape (kim cương tấm thường không có giá riêng cho shape và cut)";

    }
}

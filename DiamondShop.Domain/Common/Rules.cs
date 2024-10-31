using DiamondShop.Domain.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Common
{
    public static class Rules
    {
        public static Dictionary<string,object> DEFAULTS = new Dictionary<string, object>
        {
            { DiamondRule.key, DiamondRule.Default },
            { AccountRules.key, AccountRules.Default },
            { PromotionRule.key, PromotionRule.Default },
            { DiamondPriceRules.key, DiamondPriceRules.Default },
        };
        public static List<string> RULE_KEYS = new List<string>
        {
            DiamondRule.key,
            AccountRules.key,
            PromotionRule.key,
            DiamondPriceRules.key,
        };
    }
}

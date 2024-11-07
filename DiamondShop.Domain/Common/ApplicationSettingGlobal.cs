using DiamondShop.Domain.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Common
{
    public record ApplicationSettingGlobal
    {
        public DiamondRule DiamondRule { get; set; } = DiamondRule.Default;
        public AccountRules AccountRules { get; set; } = AccountRules.Default;
        public PromotionRule PromotionRule { get; set; } = PromotionRule.Default;
        public DiamondPriceRules DiamondPriceRules { get; set; } = DiamondPriceRules.Default;
        public WarrantyRules WarrantyRules { get; set; } = WarrantyRules.Default;
        public TransactionRule TransactionRule { get; set; } = TransactionRule.Default;
        public static Dictionary<string, object> DEFAULTS = new Dictionary<string, object>
        {
            { DiamondRule.key, DiamondRule.Default },
            { AccountRules.key, AccountRules.Default },
            { PromotionRule.key, PromotionRule.Default },
            { DiamondPriceRules.key, DiamondPriceRules.Default },
            { TransactionRule.key , TransactionRule.Default },
            { WarrantyRules.key , WarrantyRules.Default }
        };
        public static List<string> RULE_KEYS = new List<string>
        {
            DiamondRule.key,
            AccountRules.key,
            PromotionRule.key,
            DiamondPriceRules.key,
            TransactionRule.key,
            WarrantyRules.key
        };
    }
}

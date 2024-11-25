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
        public JewelryReviewRules JewelryReviewRules { get; set; } = JewelryReviewRules.Default;
        public JewelryModelCategoryRules JewelryModelCategoryRules { get; set; } = JewelryModelCategoryRules.Default;
        public BlogRules BlogRules { get; set; } = BlogRules.Default;
        public TransactionRule TransactionRule { get; set; } = TransactionRule.Default;
        public LoggingRules LoggingRules { get; set; } = LoggingRules.Default;
        public static Dictionary<string, object> DEFAULTS = new Dictionary<string, object>
        {
            { DiamondRule.key, DiamondRule.Default },
            { AccountRules.key, AccountRules.Default },
            { PromotionRule.key, PromotionRule.Default },
            { DiamondPriceRules.key, DiamondPriceRules.Default },
            { TransactionRule.key , TransactionRule.Default },
            { WarrantyRules.key , WarrantyRules.Default },
            { JewelryReviewRules.key, JewelryReviewRules.Default },
            { BlogRules.key, BlogRules.Default },
            { LoggingRules.key, LoggingRules.Default }
        };
        public static List<string> RULE_KEYS = new List<string>
        {
            DiamondRule.key,
            AccountRules.key,
            PromotionRule.key,
            DiamondPriceRules.key,
            TransactionRule.key,
            WarrantyRules.key,
            JewelryReviewRules.key,
            BlogRules.key,
            LoggingRules.key
        };
    }
}

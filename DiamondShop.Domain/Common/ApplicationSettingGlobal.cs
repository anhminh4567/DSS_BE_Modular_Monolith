﻿using DiamondShop.Domain.BusinessRules;
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
        public FrontendDisplayConfiguration FrontendDisplayConfiguration { get; set; } = FrontendDisplayConfiguration.Default;
        public CartModelRules CartModelRules { get; set; } = CartModelRules.Default;
        public OrderRule OrderRule { get; set; } = OrderRule.Default;
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
            { LoggingRules.key, LoggingRules.Default },
            { FrontendDisplayConfiguration.Key, FrontendDisplayConfiguration.Default },
            { CartModelRules.key, CartModelRules.Default},
            { OrderRule.key, OrderRule.Default }
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
            LoggingRules.key,
            OrderRule.key
        };
        private Dictionary<string, object > _extraSetting= new();
        public Dictionary<string, object> ExtraSettings { get => _extraSetting; }
        public void SetExtraSetting<T>(string key, T value) where T: class 
        {
            if (ExtraSettings.ContainsKey(key))
                ExtraSettings[key] = value;
            else
                ExtraSettings.Add(key, value);
        }
        public T? GetExtraSetting<T>(string key) where T : class
        {
            if (ExtraSettings.ContainsKey(key))
                return (T)ExtraSettings[key];
            else
                return null;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public static class JewelryRule
    {
        public static int MinimumItemPerPaging = 5;
    }
    public  class JewelryRules
    {
        public static JewelryRules Default = new JewelryRules();
        public const string Key = "JewelryRulesVer1";
        public int MaxLockHours { get; set; } = 48;
        public int MinimumItemPerPaging { get; set; } = 5;
    }

}

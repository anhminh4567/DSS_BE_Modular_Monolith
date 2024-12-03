using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class ShopBankAccountRules
    {
        public const string Key = "ShopBankAccountRulesVer1";
        public static ShopBankAccountRules Default = new ShopBankAccountRules();
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string BankBin { get; set; } = string.Empty;
        
    }
}

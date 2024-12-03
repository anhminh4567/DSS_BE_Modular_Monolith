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
        public string AccountNumber { get; set; } = "777777";
        public string AccountName { get; set; } = "TRAN DINH ANH MINH";
        public string BankBin { get; set; } = "970416";
        public string BankName { get; set; } = "ACB";
        
    }
}

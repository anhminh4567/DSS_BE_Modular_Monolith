using DiamondShop.Commons;
using DiamondShop.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class ShopBankAccountRules
    {
        public const string BANK_RULE_FOLDERS = "ShopBank";
        public const string BANK_QR_FOLDERS = BANK_RULE_FOLDERS+ "/" +"BankQR";
        public const string Key = "ShopBankAccountRulesVer2";
        public static ShopBankAccountRules Default = new ShopBankAccountRules();
        public string AccountNumber { get; set; } = "777777";
        public string AccountName { get; set; } = "TRAN DINH ANH MINH";
        public string BankBin { get; set; } = "970416";
        public string BankName { get; set; } = "ACB";
        public Media? BankQr { get; set; } 
        public static class RuleErrors
        {
            public static ConflictError QrExisted = new ConflictError("mã qr đã tồn tại");
        }
    }
}

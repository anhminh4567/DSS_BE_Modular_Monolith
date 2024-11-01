using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class AccountRules
    {
        public static AccountRules Default = new AccountRules();
        public static string key = "AccountRules";
        public static string Type = typeof(AccountRules).AssemblyQualifiedName;
        public int MaxAddress { get; set; } = 5;
    }
}

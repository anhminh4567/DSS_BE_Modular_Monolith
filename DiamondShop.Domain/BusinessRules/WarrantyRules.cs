using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class WarrantyRules
    {
        public static WarrantyRules Default = new WarrantyRules();
        public static string Type = typeof(WarrantyRules).AssemblyQualifiedName;
        public static string key = "WarrantyRule";

        public int THREE_MONTHS = 3;
        public int ONE_YEAR = 12;
        public int FIVE_YEARS = 12 * 5;
        public List<string> DEFAULT_CODE = new List<string>()
        {
            nameof(THREE_MONTHS),
            nameof(ONE_YEAR),
            nameof(FIVE_YEARS),
        };
    }
}

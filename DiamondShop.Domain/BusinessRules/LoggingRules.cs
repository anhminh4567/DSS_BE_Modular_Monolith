using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class LoggingRules
    {
        public static LoggingRules Default = new LoggingRules();
        public static string Type = typeof(LoggingRules).AssemblyQualifiedName;
        public static string key = "LoggingRules1";
        public int MaxImageAmount { get; set; } = 3;
        public int MaxLogCharacter { get; set; } = 2000;
    }
}

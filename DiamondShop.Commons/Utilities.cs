using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Commons
{
    public static class Utilities
    {
        public static string Capitalize(string sequence)
        {
            return sequence.Substring(0, 1).ToUpper() + sequence.Substring(1);
        }
    }
}

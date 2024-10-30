using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public static class JewelryModelRule
    {
        public static int MaximumSideDiamondOption = 5;
        public static int MaximumMainDiamond = 4;
        public static int ModelPerQuery = 3;
        public static int MinimumModelPerPaging = 20;
    }
}

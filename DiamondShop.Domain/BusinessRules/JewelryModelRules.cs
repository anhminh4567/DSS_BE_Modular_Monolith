using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class JewelryModelRules
    {
        public static JewelryModelRules Default = new JewelryModelRules();
        public static string Type = typeof(JewelryModelRules).AssemblyQualifiedName;
        public static string key = "JewelryModelRules";
        public JewelryModelRules() { }
        public int MaximumSideDiamondOption = 5;
        public int MaximumMainDiamond = 3;
        public int ModelPerQuery = 3;
        public int MinimumItemPerPaging = 20;
    }
}

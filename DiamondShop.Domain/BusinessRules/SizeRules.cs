using DiamondShop.Domain.Models.JewelryModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class SizeRules
    {
        public static SizeRules Default = new SizeRules();
        public SizeRules() { }
        public string key = "SizeRulesV1";
        public string Type = typeof(SizeRules).AssemblyQualifiedName;

        public int MinSizeMilimeter = 1; 
        public int MaxSizeMilimeter = 55;

        public int MinSizeCentimeter = 40;
        public int MaxSizeCentimeter = 45;

    }
}

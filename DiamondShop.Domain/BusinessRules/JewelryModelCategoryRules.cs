using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class JewelryModelCategoryRules
    {
        public static JewelryModelCategoryRules Default = new JewelryModelCategoryRules();
        public static string Type = typeof(JewelryModelCategoryRules).AssemblyQualifiedName;
        public static string key = nameof(JewelryModelCategoryRules);
        public List<string> DefaultCategories = new()
        {
            "Ring",
            "Necklace",
            "Bracelet",
            "Earring",
        };
    }
}

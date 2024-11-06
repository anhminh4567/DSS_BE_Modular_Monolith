using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds.Enums
{
    public enum Polish
    {
        Poor = 1, Fair = 2, Good = 3, Very_Good = 4, Excellent = 5,
    }
    public static class PolishHelper
    {
        public static string GetPolishName(this Polish polish)
        {
            return polish switch
            {
                Polish.Poor => "Poor",
                Polish.Fair => "Fair",
                Polish.Good => "Good",
                Polish.Very_Good => "Very Good",
                Polish.Excellent => "Excellent",
                _ => throw new ArgumentOutOfRangeException(nameof(polish), polish, null),
            };
        }
        public static List<Polish> GetPolishList()
        {
            return Enum.GetValues(typeof(Polish)).Cast<Polish>().ToList();
        }
    }
}

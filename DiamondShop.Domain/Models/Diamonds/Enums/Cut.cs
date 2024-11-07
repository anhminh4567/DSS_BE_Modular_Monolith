using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds.Enums
{
    public enum Cut
    {
        Good = 1, Very_Good = 2 , Ideal = 3
    }
    public static class CutHelper
    {
        public static string GetCutName(Cut cut)
        {
            return cut switch
            {
                Cut.Good => "Good",
                Cut.Very_Good => "Very Good",
                Cut.Ideal => "Ideal",
                _ => throw new ArgumentOutOfRangeException(nameof(cut), cut, null)
            };
        }
        public static List<Cut> GetCutList() 
        {
            return Enum.GetValues(typeof(Cut)).Cast<Cut>().ToList();
        }
    }

}

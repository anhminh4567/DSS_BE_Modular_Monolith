using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds.Enums
{
    public enum Culet
    {
        None = 1, Very_Small = 2, Small = 3, Medium = 4, Slightly_Large = 5, Large = 6 , Very_Large = 7, Extremely_Large = 8
    }
    public static class CuletHelper
    {
        public static List<Culet> GetCuletsList()
        {
            return Enum.GetValues(typeof(Culet)).Cast<Culet>().ToList();
        }
    }
}

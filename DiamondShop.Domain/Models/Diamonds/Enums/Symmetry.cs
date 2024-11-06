using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds.Enums
{
    public enum Symmetry
    {
        Poor = 1, Fair = 2, Good = 3, Very_Good = 4, Excellent = 5,
    }
    public static class SymmetryHelper
    {
        public static string GetSymmetryName(this Symmetry symmetry)
        {
            return symmetry switch
            {
                Symmetry.Poor => "Poor",
                Symmetry.Fair => "Fair",
                Symmetry.Good => "Good",
                Symmetry.Very_Good => "Very Good",
                Symmetry.Excellent => "Excellent",
                _ => throw new ArgumentOutOfRangeException(nameof(symmetry), symmetry, null),
            };
        }
        public static List<Symmetry> GetSymmetryList()
        {
            return Enum.GetValues(typeof(Symmetry)).Cast<Symmetry>().ToList();
        }
    }
}

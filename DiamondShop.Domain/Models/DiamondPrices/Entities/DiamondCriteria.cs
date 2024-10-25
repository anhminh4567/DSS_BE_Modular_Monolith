using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DiamondPrices.Entities
{
    public class DiamondCriteria : Entity<DiamondCriteriaId>
    {
        public Cut Cut { get; set; }
        public Clarity Clarity { get; set; }
        public Color Color { get; set; }
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
        public bool? IsLabGrown { get; set; }
        public static DiamondCriteria Create(Cut cut, Clarity clarity, Color color, float fromCarat ,float toCarat)
        {
            if(fromCarat > toCarat)
            {
                throw new ArgumentException("from carat is greater than to carat");
            }
            return new DiamondCriteria
            {
                Id = DiamondCriteriaId.Create(),
                Cut = cut,
                Clarity = clarity,
                Color = color,
                CaratFrom = fromCarat,
                CaratTo = toCarat,
                IsLabGrown = null,
            };
        }
        private DiamondCriteria() { }
    }
}

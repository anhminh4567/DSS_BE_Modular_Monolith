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
        public DiamondCriteria() { }
    }
}

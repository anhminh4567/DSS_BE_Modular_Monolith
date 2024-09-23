using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DiamondPrices
{
    public class DiamondPrice
    {
        public DiamondShapeId ShapeId { get; set; }
        public DiamondShape Shape { get; set; }
        public DiamondCriteriaId CriteriaId { get; set; }
        public DiamondCriteria Criteria { get; set; }
        public decimal Price { get; set; }
        public DiamondPrice() { }
    }
}

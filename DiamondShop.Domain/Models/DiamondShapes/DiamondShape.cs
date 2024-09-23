using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DiamondShapes
{
    public class DiamondShape : Entity<DiamondShapeId> , IAggregateRoot
    {
        public string Shape { get; private set; }
        public DiamondShape() { }
        public static DiamondShape Create(string shape)
        {
            return new DiamondShape() 
            {
                Id = DiamondShapeId.Create(),
                Shape = shape
            };
        }
        public void Update(string shape)
        {
            Shape = shape;
        }
    }
}

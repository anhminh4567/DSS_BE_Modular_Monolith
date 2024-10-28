using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
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
        public List<PromoReqShape> PromoReqShapes { get; set; } = new();

        public DiamondShape() { }
        public static DiamondShape AnyShape = new DiamondShape
        {
            Id = DiamondShapeId.Parse("-1"),
            Shape = "Any"
        };
        public static DiamondShape Create(string shape, DiamondShapeId? givenId = null)
        {
            return new DiamondShape() 
            {
                Id = givenId is null ? DiamondShapeId.Create() : givenId,
                Shape = shape
            };
        }
        public void Update(string shape)
        {
            Shape = shape;
        }

    }
}

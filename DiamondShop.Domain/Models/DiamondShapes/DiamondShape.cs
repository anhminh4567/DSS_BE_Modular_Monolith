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
        public static List<DiamondShape> All_Fancy_Shape { get; private set; }
        public static List<DiamondShape> All_Shape { get; private set; }
        public static DiamondShape ANY_SHAPES = Create("Any",DiamondShapeId.Parse("99"));
        public static DiamondShape FANCY_SHAPES = Create("Fancy_Shape", DiamondShapeId.Parse("98"));

        public static DiamondShape ROUND =Create("Round", DiamondShapeId.Parse(1.ToString()));
        public static DiamondShape PRINCESS = Create("Princess", DiamondShapeId.Parse(2.ToString()));
        public static DiamondShape CUSHION = Create("Cushion", DiamondShapeId.Parse(3.ToString()));
        public static DiamondShape EMERALD = Create("Emerald", DiamondShapeId.Parse(4.ToString()));
        public static DiamondShape OVAL = Create("Oval", DiamondShapeId.Parse(5.ToString()));
        public static DiamondShape RADIANT = Create("Radiant", DiamondShapeId.Parse(6.ToString()));
        public static DiamondShape ASSCHER = Create("Asscher", DiamondShapeId.Parse(7.ToString()));
        public static DiamondShape MARQUISE = Create("Marquise", DiamondShapeId.Parse(8.ToString()));
        public static DiamondShape HEART = Create("Heart", DiamondShapeId.Parse(9.ToString()));
        public static DiamondShape PEAR = Create("Pear", DiamondShapeId.Parse(10.ToString()));
        static DiamondShape()
        {
            // Initialize the static lists here
            All_Fancy_Shape = new List<DiamondShape>
            {
                PRINCESS, CUSHION, EMERALD, OVAL, RADIANT, ASSCHER, MARQUISE, HEART, PEAR,FANCY_SHAPES
            };
            All_Shape = new List<DiamondShape>(All_Fancy_Shape) { ROUND };
        }

        public string Shape { get; private set; }
        public List<PromoReqShape> PromoReqShapes { get; set; } = new();

        public DiamondShape() { }
        public static DiamondShape Create(string shape, DiamondShapeId? givenId = null)
        {
            return new DiamondShape() 
            {
                Id = givenId is null ? DiamondShapeId.Create() : givenId,
                Shape = shape
            };
        }
        public static bool IsFancyShape(DiamondShapeId shapeId)
        {
            return All_Fancy_Shape.Any(x => x.Id == shapeId);
        }
        public void Update(string shape)
        {
            Shape = shape;
        }

    }
}

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
        //given id is for shape, because we might not change this often, the id is predictable
        //and should be as short as possible 
        //ex: id should be aa,bb,cc or number int 1, 2,3 
        //for insert randomness of id, can use DateTime.utcNow.ticks.toString() to get a number string 
        //with timeStamp, for query purpose
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

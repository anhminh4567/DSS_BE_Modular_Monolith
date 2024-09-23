using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DiamondShapes.ValueObjects
{
    public record DiamondShapeId ( string Value)
    {
        public static DiamondShapeId Parse(string id)
        {
            return new DiamondShapeId(id) { Value = id };
        }
        public static DiamondShapeId Create()
        {
            return new DiamondShapeId(Guid.NewGuid().ToString());
        }
    }
}

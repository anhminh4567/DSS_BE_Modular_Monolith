using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.MainDiamonds.Commands.CompareDiamondShape
{
    public class DiamondShapeCaratHolder
    {
        public DiamondShapeId ShapeId { get; set; }
        public float Carat { get; set; }

        public DiamondShapeCaratHolder(DiamondShapeId shapeId, float carat)
        {
            ShapeId = shapeId;
            Carat = carat;
        }
    }
}

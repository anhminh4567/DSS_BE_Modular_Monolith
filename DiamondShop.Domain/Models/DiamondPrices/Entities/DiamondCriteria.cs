using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DiamondPrices.Entities
{
    public class DiamondCriteria : Entity<DiamondCriteriaId>
    {
        public DiamondShapeId ShapeId { get; set; }
        public DiamondShape Shape { get; set; }
        public Cut? Cut { get; set; }
        public Clarity? Clarity { get; set; }
        public Color? Color { get; set; }
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
        public bool? IsLabGrown { get; set; }
        public bool? IsSideDiamond { get; set; } = false;
        public List<DiamondPrice> DiamondPrices { get; set; } = new();
        public static DiamondCriteria Create(Cut? cut, Clarity clarity, Color color, float fromCarat ,float toCarat, DiamondShape shape)
        {
            ArgumentNullException.ThrowIfNull(clarity);
            ArgumentNullException.ThrowIfNull(color);
            if (fromCarat > toCarat)
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
                IsSideDiamond = false,
                ShapeId = shape.Id,
            };
        }
        public static DiamondCriteria CreateSideDiamondCriteria(float fromCarat, float toCarat , Clarity clarity, Color color, DiamondShape diamondShape)
        {
            if (fromCarat > toCarat)
            {
                throw new ArgumentException("from carat is greater than to carat");
            }
            return new DiamondCriteria
            {
                Id = DiamondCriteriaId.Create(),
                Cut = null,
                Clarity = clarity,
                Color = color,
                CaratFrom = fromCarat,
                CaratTo = toCarat,
                IsLabGrown = null,
                IsSideDiamond = true,
                ShapeId = diamondShape.Id
            };
        }
        public static DiamondCriteria CreateUnknownCriteria(bool islab, bool isSide)
        {
            return new DiamondCriteria
            {
                Id = DiamondCriteriaId.Parse("-1"),
                Cut = null,
                Clarity = null,
                Color = null,
                CaratFrom = 0,
                CaratTo = 0,
                IsLabGrown = islab,
                IsSideDiamond = isSide,
            };
        }
        public void ChangeCaratRange(float caratFrom, float caratTo)
        {
            if(caratFrom >= caratTo)
            {
                throw new ArgumentException("from carat is greater than to carat");
            }
            CaratFrom = caratFrom;
            CaratTo= caratTo;
        }
        private DiamondCriteria() { }
    }
}

using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.CustomizeRequests.Entities
{
    public class DiamondRequest 
    {
        public DiamondRequestId DiamondRequestId { get; set; }
        public CustomizeRequestId CustomizeRequestId { get; set; }
        public DiamondShapeId? DiamondShapeId { get; set; }
        public DiamondShape? DiamondShape { get; set; }
        public DiamondId? DiamondId { get; set; }
        public Diamond? Diamond { get; set; }
        public Clarity ClarityFrom { get; set; }    
        public Clarity ClarityTo { get; set; }    
        public Color ColorFrom { get; set; }
        public Color ColorTo { get; set; }
        public Cut CutFrom { get; set; }
        public Cut CutTo { get; set; }
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
        public bool? IsLabGrown { get; set; }
        public Polish? Polish { get; set; }
        public Symmetry? Symmetry { get; set; }
        public Girdle? Girdle { get; set; }
        public Culet? Culet { get; set; }
        public static DiamondRequest Create(CustomizeRequestId customizeRequestId, DiamondShapeId? diamondShapeId, Clarity clarityFrom, Clarity clarityTo, Color colorFrom, Color colorTo, Cut cutFrom, Cut cutTo, float caratFrom, float caratTo, bool? isLabGrown, Polish? polish, Symmetry? symmetry, Girdle? girdle, Culet? culet)
        {
            return new DiamondRequest()
            {
                DiamondRequestId = DiamondRequestId.Create(),
                DiamondShapeId = diamondShapeId,
                CustomizeRequestId = customizeRequestId,
                ClarityFrom = clarityFrom,
                ClarityTo = clarityTo,
                ColorFrom = colorFrom,
                ColorTo = colorTo,
                CutFrom = cutFrom,
                CutTo = cutTo,
                CaratFrom = caratFrom,
                CaratTo = caratTo,
                IsLabGrown = isLabGrown,
                Polish = polish,
                Symmetry = symmetry,
                Girdle = girdle,
                Culet = culet
            };
        }
    }
}

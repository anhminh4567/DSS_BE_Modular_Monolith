using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
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
        public DiamondId? DiamondId { get; set; }
        public int Position { get; set; } 
        public Clarity? Clarity { get; set; }    
        public Color? Color { get; set; }
        public Cut? Cut { get; set; }
        public decimal? CaratFrom { get; set; }
        public decimal? CaratTo { get; set; }
        public bool? IsLabGrown { get; set; }
        public Polish? Polish { get; set; }
        public Symmetry? Symmetry { get; set; }
        public Girdle? Girdle { get; set; }
        public Culet? Culet { get; set; }
    }
}

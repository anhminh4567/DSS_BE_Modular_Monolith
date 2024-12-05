using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Application.Dtos.Responses.Diamonds;

namespace DiamondShop.Application.Dtos.Responses.CustomizeRequests
{
    public class DiamondRequestDto
    {
        public int Position { get; set; }
        public string DiamondRequestId { get; set; }
        public string CustomizeRequestId { get; set; }
        public string? DiamondShapeId { get; set; }
        public DiamondShapeDto? DiamondShape { get; set; }
        public string? DiamondId { get; set; }
        public DiamondDto? Diamond { get; set; }
        public Clarity? ClarityFrom { get; set; }
        public Clarity? ClarityTo { get; set; }
        public Color? ColorFrom { get; set; }
        public Color? ColorTo { get; set; }
        public Cut? CutFrom { get; set; }
        public Cut? CutTo { get; set; }
        public float? CaratFrom { get; set; }
        public float? CaratTo { get; set; }
        public bool? IsLabGrown { get; set; }
        public Polish? Polish { get; set; }
        public Symmetry? Symmetry { get; set; }
        public Girdle? Girdle { get; set; }
        public Culet? Culet { get; set; }
    }
}

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

namespace DiamondShop.Application.Dtos.Responses.CustomizeRequest
{
    public class DiamondRequestDto
    {
        public string DiamondRequestId { get; set; }
        public string CustomizeRequestId { get; set; }
        public string? DiamondShapeId { get; set; }
        public DiamondShapeDto? DiamondShape { get; set; }
        public string? DiamondId { get; set; }
        public DiamondDto? Diamond { get; set; }
        public Clarity? Clarity { get; set; }
        public Color? Color { get; set; }
        public Cut? Cut { get; set; }
        public float? CaratFrom { get; set; }
        public float? CaratTo { get; set; }
        public bool? IsLabGrown { get; set; }
        public Polish? Polish { get; set; }
        public Symmetry? Symmetry { get; set; }
        public Girdle? Girdle { get; set; }
        public Culet? Culet { get; set; }
    }
}

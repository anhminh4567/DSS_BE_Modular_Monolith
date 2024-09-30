using DiamondShop.Domain.Models.Diamonds.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds
{
    public class DiamondDto
    {
        public string Id { get; set; }
        public string? JewelryId { get; set; }
        public string DiamondShapeId { get; set; }
        public DiamondShapeDto DiamondShape { get; set; }
        public DiamondWarrantyDto? Warranty { get; set; }
        /*public List<DiamondMedia> Medias { get; set;} = new();*/
        public Clarity Clarity { get; set; }
        public Color Color { get; set; }
        public Cut? Cut { get; set; }
        public decimal PriceOffset { get; set; }
        public float Carat { get; set; }
        public bool HasGIACert { get; set; }
        public bool IsLabDiamond { get; set; }
        public float WidthLengthRatio { get; set; }
        public float Depth { get; set; }
        public float Table { get; set; }
        public Polish Polish { get; set; }
        public Symmetry Symmetry { get; set; }
        public Girdle Girdle { get; set; }
        public Culet Culet { get; set; }
        public Fluorescence Fluorescence { get; set; }
        public string Measurement { get; set; }
    }
}

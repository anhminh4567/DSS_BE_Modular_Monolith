using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Domain.Models.Diamonds.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds
{
    public  class DiamondPriceDto
    {
        public string Id { get; set; }
        public string ShapeId { get; set; }
        public string CriteriaId { get; set; }
        public DiamondCriteriaDto Criteria { get; set; }
        public DiamondShapeDto Shape { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public DiscountDto? Discount { get; set; }
        public string ForUnknownPrice { get; set; }
        public Cut? Cut { get; set; }
        public Clarity? Clarity { get; set; }
        public Color? Color { get; set; }
        public bool IsLabDiamond { get; set; }
        public bool IsSideDiamond { get; set; }
    }
}

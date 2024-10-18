using DiamondShop.Application.Dtos.Responses.Promotions;
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
        public string ShapeId { get; set; }
        public string CriteriaId { get; set; }
        public DiamondCriteriaDto Criteria { get; set; }
        public DiamondShapeDto Shape { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public DiscountDto? Discount { get; set; }
        public string ForUnknownPrice { get; set; }
        public decimal TruePrice { get; set; }
    }
}

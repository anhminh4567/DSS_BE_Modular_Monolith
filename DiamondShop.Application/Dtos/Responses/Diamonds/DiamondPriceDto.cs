using System;
using System.Collections.Generic;
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
    }
}

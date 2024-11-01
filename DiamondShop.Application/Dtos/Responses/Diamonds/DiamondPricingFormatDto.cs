using DiamondShop.Application.Dtos.Responses.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds
{
    public class DiamondPricingFormatDto
    {
        public DiamondDto Diamond { get; set; }
        public DiamondPriceDto PriceFound { get; set; }
        public decimal Price { get; set; }
        public decimal? ShapeOffSetPercentFound { get; set; }

    }
}

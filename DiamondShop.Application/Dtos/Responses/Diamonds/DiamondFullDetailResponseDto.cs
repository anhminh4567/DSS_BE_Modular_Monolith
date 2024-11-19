using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds
{
    public class DiamondFullDetailResponseDto
    {
        public DiamondDto Diamond { get; set; }
        public DiamondPriceDto PriceFounded { get; set; }
        public DiamondPriceDto PriceOffset { get; set; }
        public decimal CurrentPrice { get; set; }   
        public decimal OffsetFromPriceFounded { get; set; }
        public bool IsPriceFounded { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds
{
    public class JewelryDiamondDto
    {
        public string Id { get; set; }
        public decimal? ND_Price { get; set; }
        public decimal? SD_Price { get; set; }
        public decimal? D_Price { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public bool IsAllSideDiamondPriceKnown { get; set; }

        public bool IsAllDiamondPriceKnown { get; set; }
        public bool IsMetalPriceKnown { get; set; }
        public bool IsJewelryPriceKnown { get; set; }

        public List<DiamondDto> Diamonds { get; set; }
    }
}

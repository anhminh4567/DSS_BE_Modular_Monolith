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
        public string SerialCode { get; set; }
        public decimal? ND_Price { get; set; }
        public decimal? SD_Price { get; set; }
        public decimal? D_Price { get; set; }
        public List<DiamondDto> Diamonds { get; set; }
    }
}

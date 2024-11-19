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
        //public DiamondDto? Diamond { get; set; }
        public DiamondPriceDto PriceFound { get; set; }
        public DiamondCriteriaDto CriteraFound { get; set; }
        public decimal Price { get; set; }
        public decimal CorrectPrice { get; set; }
        public decimal CurrentGivenOffset { get; set; }
        public bool IsPriceKnown { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
        public decimal SuggestedOffsetTobeAdded { get; set; }
    }
}

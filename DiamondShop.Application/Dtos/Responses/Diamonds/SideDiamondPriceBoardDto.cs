using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds
{
    public class SideDiamondPriceBoardDto
    {
        public DiamondShapeDto DiamondShape { get; set; }
        public List<SideDiamondPriceRow> SideDiamondPriceRows { get; set; } = new();
        public void FullAllRowsWithUnknownPrice(List<(float CaratFrom, float CaratTo,DiamondCriteria criteria)> values)
        {
            foreach(var value in values)
            {
                SideDiamondPriceRows.Add(new SideDiamondPriceRow
                {
                    CaratFrom = value.CaratFrom,
                    CaratTo = value.CaratTo,
                    DiamondCriteria = new DiamondCriteriaDto
                    {
                        Id = value.criteria.Id.Value,
                        CaratFrom = value.CaratFrom,
                        CaratTo = value.CaratTo,
                    }
                });
            }
        }
        public void MapPrice(DiamondPrice price)
        {
            var criteria = price.Criteria;
            var caratFrom = criteria.CaratFrom;
            var caratTo = criteria.CaratTo;
            foreach (var row in SideDiamondPriceRows)
            {
                if (row.CaratFrom == caratFrom && row.CaratTo == caratTo)
                {
                    row.DiamondPrice = new SideDiamondPriceCellDto()
                    {
                        Price = price.Price,
                    };
                    row.DiamondCriteria = new DiamondCriteriaDto
                    {
                        Id = criteria.Id.Value,
                        Cut = criteria.Cut,
                        Clarity = criteria.Clarity,
                        Color = criteria.Color,
                        CaratFrom = criteria.CaratFrom,
                        CaratTo = criteria.CaratTo,
                    };
                }
            }
        }
    }
    public class SideDiamondPriceRow
    {
        public float CaratFrom { get; set; } = 0;
        public float CaratTo { get; set; } = 0;
        public DiamondCriteriaDto DiamondCriteria { get; set; }
        public SideDiamondPriceCellDto DiamondPrice { get; set; } = new();
    }
    public class SideDiamondPriceCellDto
    {
        public decimal Price { get; set; } = -1;
        public bool IsPriceKnown => Price > 0;
    }
}

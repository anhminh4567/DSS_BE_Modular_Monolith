using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds
{
    public class DiamondPriceBoardDto
    {
        public DiamondShapeDto Shape { get; set; }
        public List<(float CaratFrom, float CaratTo)> UncoveredTableCaratRange { get; set; } = new();
        public Cut MainCut { get; set; }
        public bool IsLabDiamondBoardPrices { get; set; }
        public bool IsSideDiamondBoardPrices { get; set; }
        public List<DiamondPriceTableDto> PriceTables { get; set; } = new();
        public static DiamondPriceBoardDto Create() => new DiamondPriceBoardDto() { IsSideDiamondBoardPrices = false };

    }
    public class DiamondPriceTableDto
    {
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
        public Dictionary<Color, int> ColorRange { get; set; } = new();
        public Dictionary<Clarity, int> ClarityRange { get; set; } = new();
        //public List<DiamondPriceCellDataDto> PriceCells { get; set; } = new();
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public List<DiamondCriteria> GroupedCriteria { get; set; }
        public DiamondPriceCellDataDto[,] CellMatrix { get; set; }
        public bool IsAnyPriceUncover => CellMatrix.Cast<DiamondPriceCellDataDto>().Any(x => x.IsPriceKnown == false);
        public void FillAllCellsWithUnknonwPrice()
        {
            foreach (var color in ColorRange.Keys)
            {
                foreach (var clarity in ClarityRange.Keys)
                {
                    var colorIndex = (int)color - 1;
                    var clarityIndex = (int)clarity - 1;
                    var getCriteria = GroupedCriteria.First(x => x.Color == color && x.Clarity == clarity);
                    CellMatrix[colorIndex,clarityIndex] = new DiamondPriceCellDataDto
                    {
                        CriteriaId = getCriteria.Id.Value,
                        Color = color,
                        Clarity = clarity,
                        Price = -1,
                    };
                }
            }
        }
        public void MapPriceToCells(List<DiamondPrice> prices)
        {
            foreach (var price in prices)
            {
                var criteria = price.Criteria;
                var colorIndex = (int)criteria.Color - 1;
                var clarityIndex = (int)criteria.Clarity - 1;
                CellMatrix[colorIndex, clarityIndex].Price = price.Price;
            }
        }
        //public void FillMissingCells()
        //{
        //    var allColors = ColorRange.Select(x => x.Key).ToList();//.Where(c => c != Color.Missing);
        //    var allClarities = ClarityRange.Select(x => x.Key).ToList();

        //    // Check each color and clarity combination
        //    foreach (var color in allColors)
        //    {
        //        foreach (var clarity in allClarities)
        //        {
        //            bool exists = PriceCells.Any(cell => cell.Color == color && cell.Clarity == clarity);
        //            if (!exists)
        //            {
        //                // If it doesn't exist, add a missing cell
        //                PriceCells.Add(new DiamondPriceCellDataDto
        //                {
        //                    Color = color,
        //                    Clarity = clarity,
        //                    Price = -1,
        //                });
        //            }
        //        }
        //    }
        //}
    }
    public class DiamondPriceRowDto
    {
        public Color Color { get; set; }
        public List<DiamondPriceCellDataDto> PriceCells { get; set; } = new();
    }
    public class DiamondPriceCellDataDto
    {
        public string? CriteriaId { get; set; }
        public Cut? Cut { get; set; }
        public Color Color { get; set; }
        public Clarity Clarity { get; set; }
        public decimal Price { get; set; } = -1;
        public bool IsPriceKnown => Price > 0;
    }
}

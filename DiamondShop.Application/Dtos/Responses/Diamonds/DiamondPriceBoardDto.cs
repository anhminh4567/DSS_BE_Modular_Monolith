using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Domain.Models.Diamonds.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds
{
    public class DiamondPriceBoardDto
    {
        public DiamondShapeDto Shape { get; set; }
        public Cut MainCut { get; set; }
        public bool IsLabDiamondBoardPrices { get; set; }
        public bool IsSideDiamondBoardPrices { get; set; }
        public List<DiamondPriceTableDto> PriceTables { get; set; } = new();
        public static DiamondPriceBoardDto Create() => new DiamondPriceBoardDto() { IsSideDiamondBoardPrices = false };
        public static DiamondPriceBoardDto CreateSideDiamondBoard() => new DiamondPriceBoardDto { IsSideDiamondBoardPrices = true };

    }
    public class DiamondPriceTableDto
    {
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
        public Dictionary<int, string> ColorRange { get; set; } = new();
        public Dictionary<int, string> ClarityRange { get; set; } = new();
        public List<DiamondPriceCellDataDto> PriceCells { get; set; } = new();
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

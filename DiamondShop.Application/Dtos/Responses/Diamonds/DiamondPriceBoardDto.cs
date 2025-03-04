﻿using Azure.Storage.Blobs.Models;
using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Services.Implementations;
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
        public List<(float CaratFrom, float CaratTo)> MissingRange { get; set; } = new();
        public Cut MainCut { get; set; }
        public bool IsLabDiamondBoardPrices { get; set; }
        public bool IsSideDiamondBoardPrices { get; set; }
        public List<DiamondPriceTableDto> PriceTables { get; set; } = new();
        public static DiamondPriceBoardDto Create() => new DiamondPriceBoardDto() { IsSideDiamondBoardPrices = false };
        public void FindMissingGaps()
        {
            var ranges = PriceTables.Select(x => (x.CaratFrom, x.CaratTo)).ToList();
            // Handle edge cases
            if (ranges == null || ranges.Count < 2)
                return;// new List<(float CaratFrom, float CaratTo)>();
            // Sort ranges by the "From" value
            var sortedRanges = ranges.OrderBy(r => r.CaratFrom).ToList();
            var missingGaps = new List<(float CaratFrom, float CaratTo)>();

            for (int i = 0; i < sortedRanges.Count - 1; i++)
            {
                var current = sortedRanges[i];
                var next = sortedRanges[i + 1];
                // Check for a gap based on a precision of 0.01
                if (IsSideDiamondBoardPrices == false)
                {
                    if ((float)Math.Round(current.CaratTo + 0.01f, 2) < next.CaratFrom)
                    {
                        missingGaps.Add((current.CaratTo, next.CaratFrom));
                    }
                }
                else
                {
                    if ((current.CaratTo != next.CaratFrom))
                    {
                        missingGaps.Add((current.CaratTo, next.CaratFrom));
                    }
                }

            }
            MissingRange = missingGaps;
        }
    }
    public class DiamondPriceTableDto
    {
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
        public Dictionary<Color, int> ColorRange { get; set; } = new();
        public Dictionary<Clarity, int> ClarityRange { get; set; } = new();
        //public List<DiamondPriceCellDataDto> PriceCells { get; set; } = new();
        public List<DiscountDto> DiscountFounded { get; set; } = new();
        //[System.Text.Json.Serialization.JsonIgnore]
        //[Newtonsoft.Json.JsonIgnore]
        //public List<DiamondCriteria> GroupedCriteria { get; set; }
        public string CriteriaId { get; set; }
        public string? LastUpdate { get; set; }
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
                    //var getCriteria = GroupedCriteria..First(x => x.Color == color && x.Clarity == clarity);
                    CellMatrix[colorIndex, clarityIndex] = new DiamondPriceCellDataDto
                    {
                        DiamondPriceId = null,
                        Color = color,
                        Clarity = clarity,
                        ColorText = color.ToString(),
                        ClarityText = clarity.ToString(),
                        Price = -1,
                    };
                }
            }
        }
        public void MapPriceToCells(List<DiamondPrice> prices)
        {
            DateTime? mostCurrentDateTime = null;
            foreach (var price in prices)
            {
                //var criteria = price.Criteria;
                var colorIndex = (int)price.Color - 1;
                var clarityIndex = (int)price.Clarity - 1;
                CellMatrix[colorIndex, clarityIndex].Price = price.Price;
                CellMatrix[colorIndex, clarityIndex].DiamondPriceId = price.Id.Value;
                CellMatrix[colorIndex, clarityIndex].PriceStart = CellMatrix[colorIndex, clarityIndex].Price * (decimal)CaratFrom;
                CellMatrix[colorIndex, clarityIndex].PriceEnd = CellMatrix[colorIndex, clarityIndex].Price * (decimal)CaratTo;
                CellMatrix[colorIndex, clarityIndex].LastUpdate = price.UpdatedAt.ToLocalTime().ToString(DateTimeFormatingRules.DateTimeFormat);
                if(mostCurrentDateTime is null)
                {
                    mostCurrentDateTime = price.UpdatedAt;
                }
                else
                {
                    if (mostCurrentDateTime < price.UpdatedAt)
                        mostCurrentDateTime = price.UpdatedAt;
                }
            }
            if (mostCurrentDateTime != null)
                LastUpdate = mostCurrentDateTime.Value.ToLocalTime().ToString(DateTimeFormatingRules.DateTimeFormat);
            else
                LastUpdate = "chưa có thời gian cụ thể";
        }
        public void MapDiscounts(List<Discount> activeDiscount, Cut mainCut, DiamondShape shape, bool isLabDiamond)
        {
            foreach (var discount in activeDiscount)
            {
                if (discount.DiscountReq.Any(x => x.TargetType == Domain.Models.Promotions.Enum.TargetType.Diamond) is false)
                    continue;
                var diamondReq = discount.DiscountReq.Where(x => x.TargetType == Domain.Models.Promotions.Enum.TargetType.Diamond).ToList();
                foreach (var criteria in diamondReq)
                {
                    var foundedShape = criteria.PromoReqShapes.FirstOrDefault(x => x.ShapeId == shape.Id);
                    if (foundedShape == null)
                        continue;
                    if ((criteria.CaratFrom <= CaratTo && criteria.CaratTo >= CaratFrom) is false)
                        continue;
                    //if (criteria.CutFrom > mainCut || criteria.CutTo < mainCut)
                    //    continue;
                    if (DiamondServices.ValidateOrigin(criteria.DiamondOrigin.Value, isLabDiamond) == false)
                        continue;
                    MapToCells(criteria, discount);
                    DiscountFounded.Add(new DiscountDto
                    {
                        Id = discount.Id.Value,
                        DiscountCode = discount.DiscountCode,
                        DiscountPercent = discount.DiscountPercent,
                        Name = discount.Name,
                        DiscountReq = new List<RequirementDto>
                        {
                            new RequirementDto
                            {
                                Id = criteria.Id.Value,
                                DiamondRequirementSpec = new DiamondSpecDto
                                {
                                    CaratFrom = criteria.CaratFrom.Value,
                                    CaratTo = criteria.CaratTo.Value,
                                    CutFrom = criteria.CutFrom.Value,
                                    CutTo = criteria.CutTo.Value,
                                    ColorFrom = criteria.ColorFrom.Value,
                                    ColorTo = criteria.ColorTo.Value,
                                    ClarityFrom = criteria.ClarityFrom.Value,
                                    ClarityTo = criteria.ClarityTo.Value,
                                    ShapesIDs = criteria.PromoReqShapes.Select(x => x.ShapeId.Value).ToArray(),
                                },
                                DiscountId = discount.Id.Value,
                            }
                        }
                    });
                }
            }
        }
        private void MapToCells(PromoReq diamondReq, Discount discount)
        {
            var colorFrom = (int)diamondReq.ColorFrom;
            var colorTo = (int)diamondReq.ColorTo;
            var clarityFrom = (int)diamondReq.ClarityFrom;
            var clarityTo = (int)diamondReq.ClarityTo;
            for (int color = colorFrom; color <= colorTo; color++)
            {
                for (int clarity = clarityFrom; clarity <= clarityTo; clarity++)
                {
                    var colorIndex = color - 1;
                    var clarityIndex = clarity - 1;
                    if (CellMatrix[colorIndex, clarityIndex].IsPriceKnown is false)
                        continue;
                    if (CellMatrix[colorIndex, clarityIndex].DiscountPercentage is not null)
                    {
                        if (CellMatrix[colorIndex, clarityIndex].DiscountPercentage < discount.DiscountPercent)
                        {
                            CellMatrix[colorIndex, clarityIndex].DiscountCode = discount.DiscountCode;
                            CellMatrix[colorIndex, clarityIndex].DiscountPercentage = discount.DiscountPercent;
                        }
                    }
                    else
                    {
                        CellMatrix[colorIndex, clarityIndex].DiscountCode = discount.DiscountCode;
                        CellMatrix[colorIndex, clarityIndex].DiscountPercentage = discount.DiscountPercent;
                    }
                }
            }
        }
    }
    public class DiamondPriceRowDto
    {
        public Color Color { get; set; }
        public List<DiamondPriceCellDataDto> PriceCells { get; set; } = new();
    }
    public class DiamondPriceCellDataDto
    {
        public string? DiamondPriceId { get; set; }
        public Color Color { get; set; }
        public Clarity Clarity { get; set; }
        public string ColorText { get; set; }
        public string ClarityText { get; set; }
        public decimal Price { get; set; } = -1;
        public bool IsPriceKnown => Price > 0;
        //public decimal OffsetFromExellentCut { get; set; } = +0m;
        public string? DiscountCode { get; set; }
        public int? DiscountPercentage { get; set; }
        public string? LastUpdate { get; set; }
        public decimal? PriceStart { get; set; }
        public decimal? PriceEnd { get; set; }
    }
}

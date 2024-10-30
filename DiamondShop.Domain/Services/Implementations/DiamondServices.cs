using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    public class DiamondServices : IDiamondServices
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;

        public DiamondServices(IDiamondPriceRepository diamondPriceRepository)
        {
            _diamondPriceRepository = diamondPriceRepository;
        }

        public static Discount? AssignDiamondDiscountGlobal(Diamond diamond, List<Discount> discounts)
        {
            ArgumentNullException.ThrowIfNull(diamond.DiamondPrice);
            if (diamond.DiamondPrice.ForUnknownPrice != null)// means price is unknown
            {
                return diamond.Discount;
            }
            foreach (var discount in discounts)
            {
                foreach (var req in discount.DiscountReq)
                {
                    if (req.TargetType != Models.Promotions.Enum.TargetType.Diamond)
                        continue;
                    if (ValidateDiamond4CGlobal(diamond, req.CaratFrom.Value, req.CaratTo.Value, req.ColorFrom.Value, req.ColorTo.Value, req.ClarityFrom.Value, req.ClarityTo.Value, req.CutFrom.Value, req.CutTo.Value))
                    {
                        var discountValue = discount.DiscountPercent;
                        if (diamond.Discount != null && discountValue > diamond.Discount.DiscountPercent)
                        {
                            diamond.Discount = discount;
                        }
                        else if (diamond.Discount != null)
                        {

                        }
                        else
                        {
                            diamond.Discount = discount;
                        }
                    }
                }
            }
            if (diamond.Discount != null)
            {
                var reducedAmount = diamond.TruePrice * ((decimal)diamond.Discount.DiscountPercent / (decimal)100);
                var discountPrice = diamond.TruePrice - reducedAmount;
                var finalDiscountPrice = MoneyVndRoundUpRules.RoundAmountFromDecimal(discountPrice);
                diamond.DiscountPrice = finalDiscountPrice;
            }
            return diamond.Discount;
        }
        public Task<Discount?> AssignDiamondDiscount(Diamond diamond, List<Discount> discounts)
        {
            return Task.FromResult(AssignDiamondDiscountGlobal(diamond, discounts));
        }
        public async Task<DiamondPrice> GetDiamondPrice(Diamond diamond, List<DiamondPrice> diamondPrices)
        {
            return await GetDiamondPriceGlobal(diamond, diamondPrices);
        }
        public static async Task<DiamondPrice> GetDiamondPriceGlobal(Diamond diamond, List<DiamondPrice> diamondPrices)
        {
            foreach (var price in diamondPrices)
            {
                var isCorrectPrice = IsCorrectPrice(diamond, price);
                if (isCorrectPrice)
                {
                    decimal correctOffsetPrice = MoneyVndRoundUpRules.RoundAmountFromDecimal(price.Price * diamond.PriceOffset);
                    diamond.DiamondPrice = price;
                    diamond.SetCorrectPrice(correctOffsetPrice);
                    return price;
                }
                continue;
            }
            //throw new Exception("somehow none of the price match the diamond");
            var emptyPrice = DiamondPrice.CreateUnknownPrice(diamond.DiamondShapeId, null, diamond.IsLabDiamond);
            diamond.DiamondPrice = emptyPrice;
            diamond.SetCorrectPrice(diamond.DiamondPrice.Price);
            //emptyPrice.ForUnknownPrice = "unknown , please contact us for more information";
            return emptyPrice;
        }
        public static bool ValidateDiamond4CGlobal(Diamond diamond, float caratFrom, float caratTo, Color colorFrom, Color colorTo, Clarity clarityFrom, Clarity clarityTo, Cut cutFrom, Cut cutTo)
        {

            if (caratFrom <= diamond.Carat && caratTo >= diamond.Carat)
            {
                if (colorFrom <= diamond.Color && colorTo >= diamond.Color)
                {
                    if (clarityFrom <= diamond.Clarity && clarityTo >= diamond.Clarity)
                    {
                        if (cutFrom <= diamond.Cut && cutTo >= diamond.Cut)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool ValidateDiamond4C(Diamond diamond, float caratFrom, float caratTo, Color colorFrom, Color colorTo, Clarity clarityFrom, Clarity clarityTo, Cut cutFrom, Cut cutTo)
        {
            return ValidateDiamond4CGlobal(diamond, caratFrom, caratTo, colorFrom, colorTo, clarityFrom, clarityTo, cutFrom, cutTo);
        }

        public static bool ValidateDiamond3CGlobal(Diamond diamond, float caratFrom, float caratTo, Color colorFrom, Color colorTo, Clarity clarityFrom, Clarity clarityTo)
        {
            if (caratFrom <= diamond.Carat && caratTo >= diamond.Carat)
            {
                if (colorFrom <= diamond.Color && colorTo >= diamond.Color)
                {
                    if (clarityFrom <= diamond.Clarity && clarityTo >= diamond.Clarity)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool ValidateDiamond3C(Diamond diamond, float caratFrom, float caratTo, Color colorFrom, Color colorTo, Clarity clarityFrom, Clarity clarityTo)
        {
            return ValidateDiamond3CGlobal(diamond, caratFrom, caratTo, colorFrom, colorTo, clarityFrom, clarityTo);
        }
        public async Task<List<DiamondPrice>> GetSideDiamondPrice(JewelrySideDiamond sideDiamond)
        {
            List<DiamondPrice> diamondPrices = await _diamondPriceRepository.GetSideDiamondPriceByAverageCarat(sideDiamond.AverageCarat);
            return await GetSideDiamondPriceGlobal(sideDiamond, diamondPrices);
        }
        public static async Task<List<DiamondPrice>> GetSideDiamondPriceGlobal(JewelrySideDiamond sideDiamond, List<DiamondPrice> diamondPrices)
        {
            List<DiamondPrice> matchPrices = new();
            foreach (var price in diamondPrices)
            {
                var isMatchPrice = IsMatchSideDiamondPrice(sideDiamond, price);
                if (isMatchPrice)
                {
                    //decimal correctOffsetPrice = MoneyVndRoundUpRules.RoundAmountFromDecimal(price.Price);
                    sideDiamond.DiamondPrice.Add(price);
                }
                continue;
            }
            if (matchPrices.Count == 0)
            {
                matchPrices.Add(DiamondPrice.CreateUnknownSideDiamondPrice());
            }
            sideDiamond.DiamondPrice = matchPrices;
            sideDiamond.AveragePrice = matchPrices.Average(p => p.Price);
            //emptyPrice.ForUnknownPrice = "unknown , please contact us for more information";
            return sideDiamond.DiamondPrice;
        }
        private static bool IsCorrectPrice(Diamond diamond, DiamondPrice price)
        {
            if (diamond.DiamondShape.Id != price.ShapeId)
            {
                return false;
            }
            var criteria = price.Criteria;
            if (
                 //diamond.Cut == criteria.Cut &&
                 diamond.Color == criteria.Color
                && diamond.Clarity == criteria.Clarity
                && diamond.Carat < criteria.CaratTo
                && diamond.Carat >= criteria.CaratFrom
                && diamond.IsLabDiamond == price.IsLabDiamond)
            {
                return true;
            }
            return false;
        }
        private static bool IsMatchSideDiamondPrice(JewelrySideDiamond sideDiamond, DiamondPrice price)
        {
            bool isColorInRange = price.Criteria.Color >= sideDiamond.ColorMin && price.Criteria.Color <= sideDiamond.ColorMax;
            bool isClarityInRange = price.Criteria.Clarity >= sideDiamond.ClarityMin && price.Criteria.Clarity <= sideDiamond.ClarityMax;
            bool isCaratInRange = price.Criteria.CaratTo > sideDiamond.AverageCarat && price.Criteria.CaratFrom <= sideDiamond.AverageCarat;
            // all side diamond should be lab diamond
            //&& price.IsLabDiamond
            return isColorInRange && isClarityInRange && isCaratInRange && price.IsSideDiamond;
        }

        public async Task<List<DiamondPrice>> GetSideDiamondPrice(SideDiamondOpt sideDiamondOption)
        {
            List<DiamondPrice> diamondPrices = await _diamondPriceRepository.GetSideDiamondPriceByAverageCarat(sideDiamondOption.AverageCarat);
            JewelrySideDiamond fakeDiamond = JewelrySideDiamond.Create(JewelryId.Create(), sideDiamondOption.CaratWeight, sideDiamondOption.Quantity, sideDiamondOption.ColorMin, sideDiamondOption.ColorMax, sideDiamondOption.ClarityMin, sideDiamondOption.ClarityMax, sideDiamondOption.SettingType);
            var price = await GetSideDiamondPriceGlobal(fakeDiamond, diamondPrices);
            return price;
        }
    }
}

using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
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
using System.Threading;
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
                diamond.AssignDiscount(diamond.Discount,reducedAmount);
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
                    // day moi la cach tinh gia dung, sua la roi uncomment
                    diamond.SetCorrectPrice( price.Price);
                    //diamond.SetCorrectPrice(correctOffsetPrice);
                    return price;
                }
                continue;
            }
            var emptyPrice = DiamondPrice.CreateUnknownPrice(diamond.DiamondShapeId, null, diamond.IsLabDiamond);
            diamond.DiamondPrice = emptyPrice;
            diamond.SetCorrectPrice(diamond.DiamondPrice.Price);
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
        public async Task<DiamondPrice> GetSideDiamondPrice(JewelrySideDiamond sideDiamond)
        {
            List<DiamondPrice> diamondPrices = await _diamondPriceRepository.GetSideDiamondPriceByAverageCarat(sideDiamond.AverageCarat);
            return await GetSideDiamondPriceGlobal(sideDiamond, diamondPrices);
        }
        public static async Task<DiamondPrice> GetSideDiamondPriceGlobal(JewelrySideDiamond sideDiamond, List<DiamondPrice> diamondPrices)
        {
            // List<DiamondPrice> matchPrices = new();
            DiamondPrice matchPrice = null;
            foreach (var price in diamondPrices)
            {
                var isMatchPrice = IsMatchSideDiamondPrice(sideDiamond, price);
                if (isMatchPrice)
                {
                    matchPrice = price;
                }
                break;
            }
            if(matchPrice == null)
            {
                matchPrice = DiamondPrice.CreateUnknownSideDiamondPrice();
            }
            //sideDiamond.DiamondPriceFound = matchPrice;
            //sideDiamond.AveragePrice = matchPrice.Price;
            sideDiamond.SetCorrectPrice(sideDiamond.DiamondPriceFound.Price);
            return sideDiamond.DiamondPriceFound;
        }
        private static bool IsCorrectPrice(Diamond diamond, DiamondPrice price)
        {
            //if (diamond.DiamondShape.Id != price.ShapeId)
            //{
            //    return false;
            //}
            var isPriceFancyShape = DiamondShape.IsFancyShape(price.ShapeId);
            var isDiamondFancyShape = DiamondShape.IsFancyShape(diamond.DiamondShapeId);
            if(isPriceFancyShape != isDiamondFancyShape)
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
            //bool isColorInRange = price.Criteria.Color >= sideDiamond.ColorMin && price.Criteria.Color <= sideDiamond.ColorMax;
            //bool isClarityInRange = price.Criteria.Clarity >= sideDiamond.ClarityMin && price.Criteria.Clarity <= sideDiamond.ClarityMax;
            bool isCaratInRange = price.Criteria.CaratTo > sideDiamond.AverageCarat && price.Criteria.CaratFrom <= sideDiamond.AverageCarat;
            // all side diamond should be lab diamond
            //&& price.IsLabDiamond
            return isCaratInRange && price.IsSideDiamond;
        }

        public async Task<DiamondPrice> GetSideDiamondPrice(SideDiamondOpt sideDiamondOption)
        {
            List<DiamondPrice> diamondPrices = await _diamondPriceRepository.GetSideDiamondPriceByAverageCarat(sideDiamondOption.AverageCarat);
            JewelrySideDiamond fakeDiamond = JewelrySideDiamond.Create(JewelryId.Create(), sideDiamondOption.CaratWeight, sideDiamondOption.Quantity, sideDiamondOption.ColorMin, sideDiamondOption.ColorMax, sideDiamondOption.ClarityMin, sideDiamondOption.ClarityMax, sideDiamondOption.SettingType);
            var price = await GetSideDiamondPriceGlobal(fakeDiamond, diamondPrices);
            sideDiamondOption.DiamondPriceFound = fakeDiamond.DiamondPriceFound;
            // found price x quantity to get true total price
            sideDiamondOption.Price = fakeDiamond.AveragePrice * sideDiamondOption.Quantity;
            return price;
        }

        public async Task<List<DiamondPrice>> GetPrice(DiamondShape? shape = null, bool? isLabDiamond = null, CancellationToken token = default)
        {
            List<DiamondPrice> result = new();
            if(shape == null)
            {
                if(isLabDiamond != null)
                {
                    var getRoundBrilliantPrice = await _diamondPriceRepository.GetPrice(false, true, token);
                    var getFancyPrice = await _diamondPriceRepository.GetPrice(true, true, token);
                    var getRoundBrilliantPriceNatural = await _diamondPriceRepository.GetPrice(false, false, token);
                    var getFancyPriceNatural = await _diamondPriceRepository.GetPrice(true, false, token);
                    result.AddRange(getRoundBrilliantPrice);
                    result.AddRange(getFancyPrice);
                    result.AddRange(getRoundBrilliantPriceNatural);
                    result.AddRange(getFancyPriceNatural);
                }
                else
                {
                    var getRound = await _diamondPriceRepository.GetPrice(true, isLabDiamond, token);
                    var getFancy = await _diamondPriceRepository.GetPrice(false, isLabDiamond, token);
                    result.AddRange(getRound);
                    result.AddRange(getFancy);
                }
            }
            else
            {
                bool isFancyShape = DiamondShape.IsFancyShape(shape.Id);
                if (isLabDiamond != null)
                {
                    var getlab = await _diamondPriceRepository.GetPrice(isFancyShape, true, token);
                    var getNatural = await _diamondPriceRepository.GetPrice(isFancyShape, false, token);
                    result.AddRange(getlab);
                    result.AddRange(getNatural);
                }
                else
                {
                    var get = await _diamondPriceRepository.GetPrice(isFancyShape, isLabDiamond, token);
                    result.AddRange(get);
                }
            }
            return result;
        }
    }
}

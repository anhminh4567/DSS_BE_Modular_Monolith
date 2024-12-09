using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ErrorMessages;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
using Microsoft.Extensions.Options;

namespace DiamondShop.Domain.Services.Implementations
{
    public class DiamondServices : IDiamondServices
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;

        public DiamondServices(IDiamondPriceRepository diamondPriceRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondRepository diamondRepository, IDiamondShapeRepository diamondShapeRepository)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _optionsMonitor = optionsMonitor;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondRepository = diamondRepository;
            _diamondShapeRepository = diamondShapeRepository;
        }

        public static Discount? AssignDiamondDiscountGlobal(Diamond diamond, List<Discount> discounts)
        {
            ArgumentNullException.ThrowIfNull(diamond.DiamondPrice);
            if (diamond.DiamondPrice.ForUnknownPrice != null)// means price is unknown
            {
                return diamond.Discount;
            }
            if (diamond.Status == Common.Enums.ProductStatus.Sold)
                return null;
            foreach (var discount in discounts)
            {
                foreach (var req in discount.DiscountReq)
                {
                    if (req.TargetType != Models.Promotions.Enum.TargetType.Diamond)
                        continue;
                    if (ValidateDiamond4CGlobal(diamond, req.CaratFrom.Value, req.CaratTo.Value, req.ColorFrom.Value, req.ColorTo.Value, req.ClarityFrom.Value, req.ClarityTo.Value, req.CutFrom.Value, req.CutTo.Value))
                    {
                        if (req.PromoReqShapes.Any(x => x.ShapeId == diamond.DiamondShapeId) is false)
                        {
                            continue;
                        }
                        if (ValidateDiamondOrigin(req.DiamondOrigin.Value, diamond) == false)
                            continue;

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
                diamond.AssignDiscount(diamond.Discount, reducedAmount);
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
            var option = _optionsMonitor.CurrentValue.DiamondRule;
            return await GetDiamondPriceGlobal(diamond, diamondPrices, option);
        }
        public static async Task<DiamondPrice> GetDiamondPriceGlobal(Diamond diamond, List<DiamondPrice> diamondPrices, DiamondRule diamondRule)
        {
            // if diamond is locked for user, and price is seted
            if (diamond.Status == Common.Enums.ProductStatus.LockForUser || diamond.Status == Common.Enums.ProductStatus.PreOrder || diamond.Status == Common.Enums.ProductStatus.Locked)
            {
                if (diamond.DefaultPrice != null && diamond.DefaultPrice > 0)
                {
                    var dealedDiamondPrice = DiamondPrice.CreateDealedLockedPriceForUser(diamond);
                    diamond.DiamondPrice = dealedDiamondPrice;
                    diamond.TruePrice = dealedDiamondPrice.Price;
                    return dealedDiamondPrice;
                }
            }
            if(diamond.Status == Common.Enums.ProductStatus.Sold)
            {
                var soldPrice = DiamondPrice.CreateSoldPrice(diamond);
                diamond.DiamondPrice = soldPrice;
                diamond.TruePrice = soldPrice.Price;
                diamond.DiscountPrice = 0;
                return soldPrice;
            }
            foreach (var price in diamondPrices)
            {
                var isCorrectPrice = IsCorrectPrice(diamond, price);
                if (isCorrectPrice)
                {
                    diamond.DiamondPrice = price;
                    // day moi la cach tinh gia dung, sua la roi uncomment
                    diamond.SetCorrectPrice(price.Price, diamondRule);
                    //diamond.SetCorrectPrice(correctOffsetPrice);
                    return price;
                }
                continue;
            }
            var emptyPrice = DiamondPrice.CreateUnknownPrice(diamond.DiamondShapeId, null, diamond.IsLabDiamond);
            diamond.DiamondPrice = emptyPrice;
            diamond.SetCorrectPrice(diamond.DiamondPrice.Price, diamondRule);
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
                        if (diamond.Cut != null)
                        {
                            if (cutFrom <= diamond.Cut && cutTo >= diamond.Cut)
                            {
                                return true;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool ValidateDiamondOrigin(DiamondOrigin origin, Diamond diamond)
        {
            return ValidateOrigin(origin, diamond.IsLabDiamond);
        }
        public static bool ValidateOrigin(DiamondOrigin origin, bool isLabDiamond)
        {
            var result = origin switch
            {
                DiamondOrigin.Natural => isLabDiamond == false,
                DiamondOrigin.Lab => isLabDiamond == true,
                DiamondOrigin.Both => true,
                _ => false,
            };
            return result;
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
            //sideDiamond.IsFancyShape, 
            List<DiamondPrice> diamondPrices = await _diamondPriceRepository.GetSideDiamondPriceByAverageCarat(sideDiamond.IsLabGrown, sideDiamond.AverageCarat);
            var diamondRule = _optionsMonitor.CurrentValue.DiamondRule;
            return await GetSideDiamondPriceGlobal(sideDiamond, diamondPrices, diamondRule);
        }
        public static async Task<List<DiamondPrice>> GetSideDiamondPriceGlobal(JewelrySideDiamond sideDiamond, List<DiamondPrice> diamondPrices, DiamondRule rule)
        {
            List<DiamondPrice> matchPrices = new();
            //DiamondPrice matchPrice = null;
            foreach (var price in diamondPrices)
            {
                var isMatchPrice = IsMatchSideDiamondPrice(sideDiamond, price);
                if (isMatchPrice)
                {
                    //matchPrice = price;
                    matchPrices.Add(price);
                }
            }
            //if(matchPrice == null)
            //{
            //    matchPrice = DiamondPrice.CreateUnknownSideDiamondPrice(sideDiamond.IsLabGrown);
            //}
            if (matchPrices.Count == 0)
            {
                matchPrices.Add(DiamondPrice.CreateUnknownSideDiamondPrice(sideDiamond.IsLabGrown));
                sideDiamond.DiamondPrice = matchPrices;
                sideDiamond.AveragePricePerCarat = MoneyVndRoundUpRules.RoundAmountFromDecimal(matchPrices.Average(x => x.Price));
            }
            else
            {
                sideDiamond.DiamondPrice = matchPrices;
                var averagePrice = matchPrices.Average(x => x.Price);
                var trueAveragePrice = Math.Clamp(averagePrice, rule.MinimalSideDiamondAveragePrice, decimal.MaxValue);
                sideDiamond.AveragePricePerCarat = MoneyVndRoundUpRules.RoundAmountFromDecimal(trueAveragePrice);
            }
            //sideDiamond.DiamondPriceFound = matchPrices;
            //sideDiamond.DiamondPrice = matchPrices;
            //sideDiamond.AveragePricePerCarat = MoneyVndRoundUpRules.RoundAmountFromDecimal(matchPrices.Average(x => x.Price));
            //sideDiamond.AveragePrice = matchPrice.Price;
            //sideDiamond.SetCorrectPrice();
            return sideDiamond.DiamondPrice;
        }
        private static bool IsCorrectPrice(Diamond diamond, DiamondPrice price)
        {
            bool isFancyShapeDiamond = DiamondShape.IsFancyShape(diamond.DiamondShapeId);
            bool isFancyShapePrice = DiamondShape.IsFancyShape(price.Criteria.ShapeId);
            if (diamond.DiamondShapeId != price.Criteria.ShapeId)
            {
                if (isFancyShapeDiamond && isFancyShapePrice == false)
                {
                    return false;
                }
                else if (isFancyShapeDiamond == false && isFancyShapePrice)
                {
                    return false;
                }
                else { }
            }
            var criteria = price.Criteria;
            if (isFancyShapeDiamond)
            {
                if (diamond.Color == price.Color
                && diamond.Clarity == price.Clarity
                && diamond.Carat <= criteria.CaratTo
                && diamond.Carat >= criteria.CaratFrom
                && diamond.IsLabDiamond == price.IsLabDiamond)
                {
                    return true;
                }
            }
            else
            {
                if (diamond.Color == price.Color
                && diamond.Clarity == price.Clarity
                && diamond.Carat <= criteria.CaratTo
                && diamond.Carat >= criteria.CaratFrom
                && diamond.IsLabDiamond == price.IsLabDiamond)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsMatchSideDiamondPrice(JewelrySideDiamond sideDiamond, DiamondPrice price)
        {
            bool isColorInRange = price.Color >= sideDiamond.ColorMin && price.Color <= sideDiamond.ColorMax;
            bool isClarityInRange = price.Clarity >= sideDiamond.ClarityMin && price.Clarity <= sideDiamond.ClarityMax;
            bool isCaratInRange = sideDiamond.IsInRange(price.Criteria.CaratFrom, price.Criteria.CaratTo);  // price.Criteria.CaratTo > sideDiamond.AverageCarat && price.Criteria.CaratFrom <= sideDiamond.AverageCarat;
            //bool isPriceForFancyShape = price.IsFancyShape && sideDiamond.IsFancyShape;
            // all side diamond should be lab diamond
            //&& price.IsLabDiamond
            return isCaratInRange && price.IsSideDiamond && isColorInRange && isClarityInRange && (sideDiamond.IsLabGrown == price.IsLabDiamond);//&& isPriceForFancyShape;
        }

        public async Task<List<DiamondPrice>> GetSideDiamondPrice(SideDiamondOpt sideDiamondOption)
        {
            //sideDiamondOption.IsFancyShape
            var rule = _optionsMonitor.CurrentValue.DiamondRule;
            List<DiamondPrice> diamondPrices = await _diamondPriceRepository.GetSideDiamondPriceByAverageCarat(sideDiamondOption.IsLabGrown, sideDiamondOption.AverageCarat);
            //JewelrySideDiamond fakeDiamond = JewelrySideDiamond.Create(JewelryId.Create(), sideDiamondOption.CaratWeight, sideDiamondOption.Quantity, sideDiamondOption.ColorMin, sideDiamondOption.ColorMax, sideDiamondOption.ClarityMin, sideDiamondOption.ClarityMax, sideDiamondOption.SettingType);
            JewelrySideDiamond fakeDiamond = JewelrySideDiamond.Create(sideDiamondOption);
            var price = await GetSideDiamondPriceGlobal(fakeDiamond, diamondPrices, rule);
            //sideDiamondOption.DiamondPriceFound = fakeDiamond.DiamondPriceFound;
            sideDiamondOption.DiamondPrice = fakeDiamond.DiamondPrice;
            sideDiamondOption.AveragePricePerCarat = MoneyVndRoundUpRules.RoundAmountFromDecimal(sideDiamondOption.DiamondPrice.Average(x => x.Price));
            // found price x quantity to get true total price
            //sideDiamondOption.TotalPrice = fakeDiamond.AveragePrice * sideDiamondOption.Quantity;
            return price;
        }

        public async Task<List<DiamondPrice>> GetPrice(Cut? cut, DiamondShape shape, bool? isLabDiamond = null, CancellationToken token = default)
        {
            List<DiamondPrice> result = new();
            bool isFancyShape = DiamondShape.IsFancyShape(shape.Id);
            if (isLabDiamond == null)
            {
                var getlab = await _diamondPriceRepository.GetPrice(cut, shape, true, token);
                var getNatural = await _diamondPriceRepository.GetPrice(cut, shape, false, token);
                result.AddRange(getlab);
                result.AddRange(getNatural);
            }
            else
            {
                var get = await _diamondPriceRepository.GetPrice(cut, shape, isLabDiamond.Value, token);
                result.AddRange(get);
            }
            //}
            return result;
        }

        public async Task<bool> IsMainDiamondFoundInCriteria(Diamond diamond)
        {
            bool isFancyShapeDiamond = DiamondShape.IsFancyShape(diamond.DiamondShapeId);
            var getShape = await _diamondShapeRepository.GetById(diamond.DiamondShapeId);
            if (getShape is null)
                throw new Exception(DiamondShapeErrors.NotFoundError.Message);
            var groupedCritera = await _diamondCriteriaRepository.GroupAllAvailableCriteria(getShape);
            var diamondCarat = diamond.Carat;
            var caratGroup = groupedCritera.Keys.ToList();
            bool foundedCriteria = false;
            foreach (var group in caratGroup)
            {
                if (group.CaratFrom <= diamondCarat && group.CaratTo >= diamondCarat)
                {
                    var criteria = groupedCritera[group];
                    if (criteria is not null)
                    {
                        foundedCriteria = true;
                    }
                }
            }
            if (foundedCriteria == false)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> IsSideDiamondFoundInCriteria(JewelrySideDiamond sideDiamond)
        {
            var groupCriteria = await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCriteria();
            var sideDiamondAvgCarat = sideDiamond.AverageCarat;
            var caratGroup = groupCriteria.Keys.ToList();
            bool foundedCriteria = false;
            foreach (var group in caratGroup)
            {
                if (sideDiamond.IsInRange(group.CaratFrom, group.CaratTo))
                //if (group.CaratFrom <= sideDiamondAvgCarat && group.CaratTo > sideDiamondAvgCarat)
                {
                    var criteria = groupCriteria[group];
                    if (criteria is not null)
                    {
                        foundedCriteria = true;
                    }
                }
            }
            if (foundedCriteria == false)
            {
                return false;
            }
            return true;
        }

        public Task<bool> IsSideDiamondFoundInCriteria(SideDiamondOpt sideDiamondOpt)
        {
            //var mappedToJewelrySideDiamond = JewelrySideDiamond.Create(JewelryId.Create(), sideDiamondOpt.CaratWeight, sideDiamondOpt.Quantity, sideDiamondOpt.ColorMin, sideDiamondOpt.ColorMax, sideDiamondOpt.ClarityMin, sideDiamondOpt.ClarityMax, sideDiamondOpt.SettingType);
            var mappedToJewelrySideDiamond = JewelrySideDiamond.Create(sideDiamondOpt);
            return IsSideDiamondFoundInCriteria(mappedToJewelrySideDiamond);
        }
    }
}

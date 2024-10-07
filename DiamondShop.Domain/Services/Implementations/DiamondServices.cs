using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Services.interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    public class DiamondServices : IDiamondServices
    {
        //private readonly ILogger<DiamondServices> _logger;

        public DiamondServices()
        {
            //_logger = logger;
        }

        public async Task<DiamondPrice> GetDiamondPrice(Diamond diamond, List<DiamondPrice> diamondPrices)
        {
            foreach (var price in diamondPrices)
            {
                var isCorrectPrice = IsCorrectPrice(diamond, price);
                if (isCorrectPrice)
                {
                    return price;
                }
                continue;
            }
            throw new Exception("somehow none of the price match the diamond");
        }

        public bool ValidateDiamond4C(Diamond diamond, float caratFrom, float caratTo, Color colorFrom, Color colorTo, Clarity clarityFrom, Clarity clarityTo, Cut cutFrom, Cut cutTo)
        {
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
        }

        private bool IsCorrectPrice(Diamond diamond, DiamondPrice price)
        {
            if (diamond.DiamondShape.Id != price.ShapeId)
            {
                return false;
            }
            var criteria = price.Criteria;
            if (diamond.Cut != criteria.Cut
                || diamond.Color != criteria.Color
                || diamond.Clarity != criteria.Clarity
                || diamond.Carat > criteria.CaratTo
                || diamond.Carat <= criteria.CaratFrom)
            {
                return false;
            }
            return true;
        }
    }
}

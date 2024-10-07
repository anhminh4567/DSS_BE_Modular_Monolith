using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IDiamondServices
    {
        //return expected price
        Task<DiamondPrice> GetDiamondPrice(Diamond diamond, List<DiamondPrice> diamondPrices);
        bool ValidateDiamond4C(Diamond diamond, float caratFrom, float caratTo, Color colorFrom, Color colorTo, Clarity clarityFrom, Clarity clarityTo, Cut cutFrom, Cut cutTo);
    }
}

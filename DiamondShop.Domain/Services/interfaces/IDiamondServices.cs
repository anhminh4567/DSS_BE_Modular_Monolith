using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
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
        Task<List<DiamondPrice>> GetSideDiamondPrice(JewelrySideDiamond sideDiamond);
        Task<List<DiamondPrice>> GetSideDiamondPrice(SideDiamondOpt sideDiamondOption);
        bool ValidateDiamond4C(Diamond diamond, float caratFrom, float caratTo, Color colorFrom, Color colorTo, Clarity clarityFrom, Clarity clarityTo, Cut cutFrom, Cut cutTo);
        bool ValidateDiamond3C(Diamond diamond, float caratFrom, float caratTo, Color colorFrom, Color colorTo, Clarity clarityFrom, Clarity clarityTo);
        Task<bool> IsMainDiamondFoundInCriteria(Diamond diamond);
        Task<bool> IsSideDiamondFoundInCriteria(JewelrySideDiamond sideDiamond);
        Task<bool> IsSideDiamondFoundInCriteria(SideDiamondOpt sideDiamondOpt);

        Task<Discount?> AssignDiamondDiscount(Diamond diamond, List<Discount> discounts);

        Task<List<DiamondPrice>> GetPrice(Cut cut, DiamondShape shape, bool? isLabDiamond = null, CancellationToken token = default);


    }
}

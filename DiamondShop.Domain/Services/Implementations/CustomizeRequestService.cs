using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Services.interfaces;

namespace DiamondShop.Domain.Services.Implementations
{
    public class CustomizeRequestService : ICustomizeRequestService
    {
        public bool IsAssigningDiamondSpecValid(DiamondRequest request, Diamond diamond)
        {
            if (request.DiamondShapeId != null && request.DiamondShapeId != diamond.DiamondShapeId)
                return false;
            if (request.ClarityFrom > diamond.Clarity || request.ClarityTo < diamond.Clarity)
                return false;
            if (request.CutFrom > diamond.Cut || request.CutTo < diamond.Cut)
                return false;
            if (request.ColorFrom > diamond.Color || request.ColorTo < diamond.Color)
                return false;
            if (request.CaratFrom > diamond.Carat || request.CaratTo < diamond.Carat)
                return false;
            if (request.IsLabGrown != null && request.IsLabGrown != diamond.IsLabDiamond)
                return false;
            if (request.Polish != null && request.Polish != diamond.Polish)
                return false;
            if (request.Symmetry != null && request.Symmetry != diamond.Symmetry)
                return false;
            if (request.Girdle != null && request.Girdle != diamond.Girdle)
                return false;
            if (request.Culet != null && request.Culet != diamond.Culet)
                return false;
            return true;
        }
    }
}

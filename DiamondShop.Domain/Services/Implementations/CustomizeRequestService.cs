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
            if (request.Clarity != null && request.Clarity != diamond.Clarity)
                return false;
            if (request.Color != null && request.Color != diamond.Color)
                return false;
            if (request.Cut != null && request.Cut != diamond.Cut)
                return false;
            if (request.CaratFrom != null && request.CaratFrom > diamond.Carat)
                return false;
            if (request.CaratTo != null && request.CaratTo < diamond.Carat)
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

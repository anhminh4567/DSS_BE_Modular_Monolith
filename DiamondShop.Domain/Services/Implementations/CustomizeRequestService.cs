using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
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
        public void SetStage(CustomizeRequest req, bool isPriced = false)
        {
            req.Stage = req.Status switch
            {
                CustomizeRequestStatus.Pending => 1,
                CustomizeRequestStatus.Priced => 3,
                CustomizeRequestStatus.Requesting => 5,
                CustomizeRequestStatus.Accepted => 7,
                CustomizeRequestStatus.Shop_Rejected => !isPriced ? 2 : 6,
                CustomizeRequestStatus.Customer_Rejected => req.Jewelry == null ? 4 : 8,
            };
        }
    }
}

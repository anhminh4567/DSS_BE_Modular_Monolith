using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.Diamonds;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface ICustomizeRequestService
    {
        bool IsAssigningDiamondSpecValid(DiamondRequest request, Diamond diamond);
        void SetStage(CustomizeRequest req, bool isPriced);
    }
}

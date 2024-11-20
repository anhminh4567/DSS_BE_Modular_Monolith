using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.Diamonds;
using FluentResults;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface ICustomizeRequestService
    {
        Result IsAssigningDiamondSpecValid(DiamondRequest request, Diamond diamond);
    }
}

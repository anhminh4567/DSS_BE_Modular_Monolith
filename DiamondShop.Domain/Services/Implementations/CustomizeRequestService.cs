using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;

namespace DiamondShop.Domain.Services.Implementations
{
    public class CustomizeRequestService : ICustomizeRequestService
    {
        public Result IsAssigningDiamondSpecValid(DiamondRequest request, Diamond diamond)
        {
            List<IError> errors = new();
            if (request.DiamondShapeId != null && request.DiamondShapeId != diamond.DiamondShapeId)
                return Result.Fail("");
            if (request.ClarityFrom > diamond.Clarity || request.ClarityTo < diamond.Clarity)
                return Result.Fail("");
            if (request.CutFrom > diamond.Cut || request.CutTo < diamond.Cut)
                return Result.Fail("");
            if (request.ColorFrom > diamond.Color || request.ColorTo < diamond.Color)
                return Result.Fail("");
            if (request.CaratFrom > diamond.Carat || request.CaratTo < diamond.Carat)
                return Result.Fail("");
            if (request.IsLabGrown != null && request.IsLabGrown != diamond.IsLabDiamond)
                return Result.Fail("");
            if (request.Polish != null && request.Polish != diamond.Polish)
                return Result.Fail("");
            if (request.Symmetry != null && request.Symmetry != diamond.Symmetry)
                return Result.Fail("");
            if (request.Girdle != null && request.Girdle != diamond.Girdle)
                return Result.Fail("");
            if (request.Culet != null && request.Culet != diamond.Culet)
                return Result.Fail("");
            return Result.Ok();
        }
    }
}

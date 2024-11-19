using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using FluentResults;
using MediatR;
namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetPaging
{
    public record GetDiamond_4C(Cut? cutFrom, Cut? cutTo, Color? colorFrom, Color? colorTo, Clarity? clarityFrom, Clarity? clarityTo, float? caratFrom, float? caratTo);
    public record GetDiamond_Details(Polish? Polish, Symmetry? Symmetry, Girdle? Girdle, Fluorescence? Fluorescence, Culet? Culet, bool isGIA = true);
    public record GetDiamond_ManagerQuery(List<ProductStatus>? diamondStatuses, int? diamondLastUpdatedRange);
    public record GetDiamondPagingQuery(bool isLab,bool? includeJewelryDiamond = false, int pageSize = 20, int start = 0, string? shapeId = "1", decimal priceStart = 0, decimal priceEnd = decimal.MaxValue,
        GetDiamond_4C? diamond_4C = null, GetDiamond_Details? diamond_Details = null, GetDiamond_ManagerQuery? GetDiamond_ManagerQuery = null) : IRequest<Result<PagingResponseDto<Diamond>>>;
}

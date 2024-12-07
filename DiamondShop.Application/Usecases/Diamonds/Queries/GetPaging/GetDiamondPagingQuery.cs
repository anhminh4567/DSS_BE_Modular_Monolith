using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetPaging
{

    public record GetDiamondPagingQuery(bool? isLab, string? shapeId, bool? includeJewelryDiamond = false, int pageSize = 20, int start = 0, decimal priceStart = 0, decimal priceEnd = decimal.MaxValue,
        GetDiamond_4C? diamond_4C = null, GetDiamond_Details? diamond_Details = null, GetDiamond_ManagerQuery? GetDiamond_ManagerQuery = null) : IRequest<Result<PagingResponseDto<Diamond>>>;
}

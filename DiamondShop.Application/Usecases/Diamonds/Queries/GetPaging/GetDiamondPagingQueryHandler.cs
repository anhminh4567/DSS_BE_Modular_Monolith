using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetPaging
{
    public record GetDiamond_4C(Cut? cutFrom, Cut? cutTo, Color? colorFrom, Color? colorTo, Clarity? clarityFrom, Clarity? clarityTo, float? caratFrom, float? caratTo);
    public record GetDiamond_Details(Polish? Polish, Symmetry? Symmetry, Girdle? Girdle, Fluorescence? Fluorescence, Culet? Culet , bool isGIA);
    public record GetDiamondPagingQuery(int pageSize, int start, 
        GetDiamond_4C diamond_4C , GetDiamond_Details diamond_Details) : IRequest<Result<PagingResponseDto<Diamond>>>;
    internal class GetDiamondPagingQueryHandler : IRequestHandler<GetDiamondPagingQuery, Result<PagingResponseDto<Diamond>>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondServices _diamondService;
        public async Task<Result<PagingResponseDto<Diamond>>> Handle(GetDiamondPagingQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out var pageSize, out int start, out var diamond_4C, out var diamond_Details);
            var query =  _diamondRepository.GetQuery();
            if(diamond_4C.cutFrom is not null || diamond_4C.cutTo is not null)
            {

            }
            if (diamond_4C.clarityFrom is not null || diamond_4C.clarityTo is not null)
            {

            }
            if (diamond_4C.colorFrom is not  null || diamond_4C.colorTo is not null)
            {

            }
            if (diamond_4C.caratFrom is not null || diamond_4C.caratTo is not null)
            {

            }

            throw new NotImplementedException();
        }
    }
}

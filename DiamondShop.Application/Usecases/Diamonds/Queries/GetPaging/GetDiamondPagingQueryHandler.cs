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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetPaging
{
    
    internal class GetDiamondPagingQueryHandler : IRequestHandler<GetDiamondPagingQuery, Result<PagingResponseDto<Diamond>>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondServices _diamondService;

        public GetDiamondPagingQueryHandler(IDiamondRepository diamondRepository, IDiamondPriceRepository diamondPriceRepository, IDiamondServices diamondService)
        {
            _diamondRepository = diamondRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondService = diamondService;
        }

        public async Task<Result<PagingResponseDto<Diamond>>> Handle(GetDiamondPagingQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int pageSize, out int start, out var diamond_4C, out var diamond_Details);
            var query = _diamondRepository.GetQuery();
            if (diamond_4C is not null)
                Filtering4C(query, diamond_4C);
            if (diamond_Details is not null) 
                FilteringDetail(query, diamond_Details);
            query.Skip(start* pageSize);
            query.Take(pageSize);
            var result = query.ToList();
            var count = _diamondRepository.GetCount();
            var totalPage = (int)Math.Ceiling((decimal)count / (decimal)pageSize);
            var response = new PagingResponseDto<Diamond>(
                totalPage: totalPage,
                currentPage: start,
                Values: result
                ) ;
            return response;
        }
        private void Filtering4C(in IQueryable<Diamond> query, GetDiamond_4C diamond_4C)
        {
            if (diamond_4C.cutFrom is not null || diamond_4C.cutTo is not null)
                _diamondRepository.QueryFilter(query, d => d.Cut >= diamond_4C.cutFrom && d.Cut <= diamond_4C.cutTo);
            if (diamond_4C.clarityFrom is not null || diamond_4C.clarityTo is not null)
                _diamondRepository.QueryFilter(query, d => d.Clarity >= diamond_4C.clarityFrom && d.Clarity <= diamond_4C.clarityTo);
            if (diamond_4C.colorFrom is not null || diamond_4C.colorTo is not null)
                _diamondRepository.QueryFilter(query, d => d.Color >= diamond_4C.colorFrom && d.Color <= diamond_4C.colorTo);
            if (diamond_4C.caratFrom is not null || diamond_4C.caratTo is not null)
                _diamondRepository.QueryFilter(query, d => d.Carat >= diamond_4C.caratFrom && d.Carat <= diamond_4C.caratTo);
        }
        private void FilteringDetail(in IQueryable<Diamond> query, GetDiamond_Details diamond_Details)
        {
            if (diamond_Details.Culet is not null)
                _diamondRepository.QueryFilter(query,d => d.Culet == diamond_Details.Culet);
            if (diamond_Details.isGIA)
                _diamondRepository.QueryFilter(query, d => d.HasGIACert == diamond_Details.isGIA);
            if (diamond_Details.Fluorescence is not null)
                _diamondRepository.QueryFilter(query, d => d.Fluorescence == diamond_Details.Fluorescence);
            if (diamond_Details.Polish is not null)
                _diamondRepository.QueryFilter(query, d => d.Polish == diamond_Details.Polish);
            if (diamond_Details.Girdle is not null)
                _diamondRepository.QueryFilter(query, d => d.Girdle == diamond_Details.Girdle);

        }
    }
}

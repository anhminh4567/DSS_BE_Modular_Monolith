using Azure;
using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using MediatR;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Queries.GetPaging
{
    public record GetPromotionPagingQuery(int start, int take, List<Status> statuses, string? code): IRequest<PagingResponseDto<Promotion>>;
    internal class GetPromotionPagingQueryHandler : IRequestHandler<GetPromotionPagingQuery, PagingResponseDto<Promotion>>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IPromotionServices _promotionServices;

        public GetPromotionPagingQueryHandler(IPromotionRepository promotionRepository, IPromotionServices promotionServices)
        {
            _promotionRepository = promotionRepository;
            _promotionServices = promotionServices;
        }

        public async Task<PagingResponseDto<Promotion>> Handle(GetPromotionPagingQuery request, CancellationToken cancellationToken)
        {
            var trueSkip = request.start * request.take;
            
            if (request.code != null)
            {
                var result = await _promotionRepository.GetContainingCode(request.code, trueSkip, request.take, cancellationToken);
                var queryCount = _promotionRepository.GetQuery();
                queryCount = _promotionRepository.QueryFilter(queryCount, x => x.PromoCode.ToUpper().Contains(request.code.ToUpper()));
                var count = queryCount.Count();
                var totalPage = (int)Math.Ceiling((double)count / request.take);
                return new PagingResponseDto<Promotion>(totalPage,request.start,result);
            }
            else
            {
                var query = _promotionRepository.GetQuery();
                query = _promotionRepository.QueryByStatuses(query, request.statuses);
                var result = query.Skip(trueSkip).Take(request.take).ToList();
                var queryCount = _promotionRepository.GetQuery();
                queryCount = _promotionRepository.QueryByStatuses(query, request.statuses);
                var count = queryCount.Count();
                var totalPage = (int)Math.Ceiling((double)count / request.take);
                return new PagingResponseDto<Promotion>(totalPage, request.start, result);
            }
        }
    }
    
}

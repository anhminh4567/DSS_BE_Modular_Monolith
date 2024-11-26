using Azure.Core;
using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Discounts.Queries.GetPaging
{
    public record GetDiscountPagingQuery(int start, int take, List<Status> statuses, string? code): IRequest<PagingResponseDto<Discount>>;
    internal class GetDiscountPagingQueryHandler : IRequestHandler<GetDiscountPagingQuery,PagingResponseDto<Discount>>
    {
        private readonly IDiscountRepository _discountRepository;

        public GetDiscountPagingQueryHandler(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<PagingResponseDto<Discount>> Handle(GetDiscountPagingQuery request, CancellationToken cancellationToken)
        {
            var trueSkip = request.start * request.take;

            if (request.code != null)
            {
                var result = await _discountRepository.GetContainingCode(request.code, trueSkip, request.take, cancellationToken);
                var queryCount = _discountRepository.GetQuery();
                queryCount = _discountRepository.QueryFilter(queryCount, x => x.DiscountCode.ToUpper().Contains(request.code.ToUpper()));
                var count = queryCount.Count();
                var totalPage = (int)Math.Ceiling((double)count / request.take);
                return new PagingResponseDto<Discount>(totalPage, request.start, result);

            }
            else
            {
                var query = _discountRepository.GetQuery();
                query = _discountRepository.QueryByStatuses(query, request.statuses);
                var result = query.Skip(trueSkip).Take(request.take).ToList();
                var queryCount = _discountRepository.GetQuery();
                queryCount = _discountRepository.QueryByStatuses(query, request.statuses);
                var count = queryCount.Count();
                var totalPage = (int)Math.Ceiling((double)count / request.take);
                return new PagingResponseDto<Discount>(totalPage, request.start, result);
            }
        }
    }
}

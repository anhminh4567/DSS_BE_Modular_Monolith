using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Discounts.Queries.GetWithFilter
{

    public record GetDiscountsWithFilterQuery(List<Status> Statuses) : IRequest<List<Discount>>;
    internal class GetDiscountsWithFilterQueryHandler : IRequestHandler<GetDiscountsWithFilterQuery, List<Discount>>
    {
        private readonly IDiscountRepository _discountRepository;

        public GetDiscountsWithFilterQueryHandler(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<List<Discount>> Handle(GetDiscountsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var query = _discountRepository.GetQuery();
            query = _discountRepository.QueryByStatuses(query, request.Statuses);
            var result = query.ToList();
            return result;
        }
    }
}

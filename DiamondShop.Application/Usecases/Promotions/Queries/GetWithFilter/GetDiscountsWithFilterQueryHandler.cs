using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Queries.GetWithFilter
{

    public record GetPromotionWithFilterQuery(List<Status> Statuses) : IRequest<List<Promotion>>;
    internal class GetPromotionWithFilterQueryHandler : IRequestHandler<GetPromotionWithFilterQuery, List<Promotion>>
    {
        private readonly IPromotionRepository _discountRepository;

        public GetPromotionWithFilterQueryHandler(IPromotionRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<List<Promotion>> Handle(GetPromotionWithFilterQuery request, CancellationToken cancellationToken)
        {
            var query = _discountRepository.GetQuery();
            query = _discountRepository.QueryByStatuses(query, request.Statuses);
            var result = query.ToList();
            return result;
        }
    }
}

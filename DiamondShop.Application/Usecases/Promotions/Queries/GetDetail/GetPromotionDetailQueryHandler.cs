using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Queries.GetDetail
{
    public record GetPromotionDetailQuery (string id) : IRequest<Result<Promotion>>;
    internal class GetPromotionDetailQueryHandler : IRequestHandler<GetPromotionDetailQuery, Result<Promotion>>
    {
        private readonly IPromotionRepository _promotionRepository;

        public GetPromotionDetailQueryHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<Result<Promotion>> Handle(GetPromotionDetailQuery request, CancellationToken cancellationToken)
        {
            var parsedId = PromotionId.Parse(request.id);
            return await _promotionRepository.GetById(parsedId);
        }
    }
}

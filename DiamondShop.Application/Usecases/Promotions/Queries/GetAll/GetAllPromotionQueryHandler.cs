using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Queries.GetAll
{
    
    public record GetAllPromotionQuery() : IRequest<List<Promotion>>;
    internal class GetAllPromotionQueryHandler : IRequestHandler<GetAllPromotionQuery, List<Promotion>>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly ILogger<GetAllPromotionQueryHandler> _logger;

        public GetAllPromotionQueryHandler(IPromotionRepository promotionRepository, ILogger<GetAllPromotionQueryHandler> logger)
        {
            _promotionRepository = promotionRepository;
            _logger = logger;
        }

        public async Task<List<Promotion>> Handle(GetAllPromotionQuery request, CancellationToken cancellationToken)
        {
            var query = _promotionRepository.GetQuery();
            query = _promotionRepository.QueryInclude(query, r => r.PromoReqs);
            query = _promotionRepository.QueryInclude(query, r => r.Gifts);
            return (query.ToList());
        }
    }
}

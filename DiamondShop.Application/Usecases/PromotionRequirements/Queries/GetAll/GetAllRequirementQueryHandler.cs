using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.PromotionRequirements.Queries.GetAll
{
    public record GetAllRequirementQuery() : IRequest<List<PromoReq>>;
    internal class GetAllRequirementQueryHandler : IRequestHandler<GetAllRequirementQuery, List<PromoReq>>
    {
        private readonly IRequirementRepository _requirementRepository;
        private readonly ILogger<GetAllRequirementQueryHandler> _logger;

        public GetAllRequirementQueryHandler(IRequirementRepository requirementRepository, ILogger<GetAllRequirementQueryHandler> logger)
        {
            _requirementRepository = requirementRepository;
            _logger = logger;
        }

        public async Task<List<PromoReq>> Handle(GetAllRequirementQuery request, CancellationToken cancellationToken)
        {
            var query = _requirementRepository.GetQuery();
            query = _requirementRepository.QueryInclude(query,r => r.PromoReqShapes);
            return (query.ToList());
        }
    }
}

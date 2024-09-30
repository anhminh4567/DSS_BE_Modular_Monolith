using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.PromotionRequirements.Queries.GetDetail
{
    public record GetRequirementDetailQuery(string requirementId) : IRequest<Result<PromoReq>>;
    internal class GetRequirementDetailQueryHandler : IRequestHandler<GetRequirementDetailQuery, Result<PromoReq>>
    {
        private readonly IRequirementRepository _requirementRepository;
        private readonly ILogger<GetRequirementDetailQueryHandler> _logger;

        public GetRequirementDetailQueryHandler(IRequirementRepository requirementRepository, ILogger<GetRequirementDetailQueryHandler> logger)
        {
            _requirementRepository = requirementRepository;
            _logger = logger;
        }

        public async Task<Result<PromoReq>> Handle(GetRequirementDetailQuery request, CancellationToken cancellationToken)
        {
            var reqId = PromoReqId.Parse(request.requirementId);
            var query = _requirementRepository.GetQuery();
            query = _requirementRepository.QueryInclude(query,r => r.PromoReqShapes);
            var result = query.FirstOrDefault(r => r.Id == reqId);
            if (result == null)
                return Result.Fail(new NotFoundError());
            return result;
        }
    }
}

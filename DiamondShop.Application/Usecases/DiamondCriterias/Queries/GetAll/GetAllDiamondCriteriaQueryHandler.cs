using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondCriterias.Queries.GetAll
{
    public record GetAllDiamondCriteriaQuery() : IRequest<List<DiamondCriteria>>;
    internal class GetAllDiamondCriteriaQueryHandler : IRequestHandler<GetAllDiamondCriteriaQuery, List<DiamondCriteria>>
    {
        private readonly ILogger<GetAllDiamondCriteriaQueryHandler> _logger;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;

        public GetAllDiamondCriteriaQueryHandler(ILogger<GetAllDiamondCriteriaQueryHandler> logger, IDiamondCriteriaRepository diamondCriteriaRepository)
        {
            _logger = logger;
            _diamondCriteriaRepository = diamondCriteriaRepository;
        }

        public async Task<List<DiamondCriteria>> Handle(GetAllDiamondCriteriaQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("call getall diamond criteria");
            var result = await _diamondCriteriaRepository.GetAll();
            return result.ToList() ;
        }
    }
}

using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetAll
{
    public record GetAllDiamondQuery() : IRequest<List<Diamond>>;
    internal class GetAllDiamondQueryHandler : IRequestHandler<GetAllDiamondQuery, List<Diamond>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly ILogger<GetAllDiamondQueryHandler> _logger;

        public GetAllDiamondQueryHandler(IDiamondRepository diamondRepository, ILogger<GetAllDiamondQueryHandler> logger)
        {
            _diamondRepository = diamondRepository;
            _logger = logger;
        }

        public async Task<List<Diamond>> Handle(GetAllDiamondQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("get all diamond");
            return (await _diamondRepository.GetAll()).ToList();
        }
    }
}

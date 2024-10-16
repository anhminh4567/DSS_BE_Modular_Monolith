using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Addresses;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Addresses.Queries.GetAllWards
{
    public record GetAllWardQuery(string wardId) : IRequest<List<Ward>>;
    internal class GetAllWardQueryHandler : IRequestHandler<GetAllWardQuery, List<Ward>>
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<GetAllWardQueryHandler> _logger;

        public GetAllWardQueryHandler(ILocationService locationService, ILogger<GetAllWardQueryHandler> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        public async Task<List<Ward>> Handle(GetAllWardQuery request, CancellationToken cancellationToken)
        {
            return _locationService.GetWards(request.wardId);
        }
    }

}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Addresses;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Addresses.Queries.GetAllDistricts
{

    public record GetAllDistrictQuery(string provinceId) : IRequest<List<District>>;
    internal class GetAllDistrictQueryHandler : IRequestHandler<GetAllDistrictQuery, List<District>>
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<GetAllDistrictQueryHandler> _logger;

        public GetAllDistrictQueryHandler(ILocationService locationService, ILogger<GetAllDistrictQueryHandler> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        public async Task<List<District>> Handle(GetAllDistrictQuery request, CancellationToken cancellationToken)
        {
            return _locationService.GetDistricts(request.provinceId);
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Addresses;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Addresses.Queries.GetAllProvince
{
    public record GetAllProvinceQuery() : IRequest<List<Province>>;
    internal class GetAllProvinceQueryHandler : IRequestHandler<GetAllProvinceQuery, List<Province>>
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<GetAllProvinceQueryHandler> _logger;

        public GetAllProvinceQueryHandler(ILocationService locationService, ILogger<GetAllProvinceQueryHandler> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        public async Task<List<Province>> Handle(GetAllProvinceQuery request, CancellationToken cancellationToken)
        {
            return _locationService.GetProvinces();
        }
    }
}

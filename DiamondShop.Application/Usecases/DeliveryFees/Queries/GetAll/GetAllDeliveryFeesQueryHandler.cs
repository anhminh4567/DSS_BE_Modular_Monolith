using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DeliveryFees.Queries.GetAll
{
    public record GetAllDeliveryFeeQuery() : IRequest<List<DeliveryFee>>;
    internal class GetAllDeliveryFeesQueryHandler : IRequestHandler<GetAllDeliveryFeeQuery, List<DeliveryFee>>
    {
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly ILogger<GetAllDeliveryFeesQueryHandler> _logger;

        public GetAllDeliveryFeesQueryHandler(IDeliveryFeeRepository deliveryFeeRepository, ILogger<GetAllDeliveryFeesQueryHandler> logger)
        {
            _deliveryFeeRepository = deliveryFeeRepository;
            _logger = logger;
        }

        public async Task<List<DeliveryFee>> Handle(GetAllDeliveryFeeQuery request, CancellationToken cancellationToken)
        {
            var query = _deliveryFeeRepository.GetQuery();
            query = query.OrderBy(s => s.FromKm).ThenBy(s => s.Cost);
            var result = query.ToList();
            _logger.LogInformation("GetAll DeliveryFees is called with total " + result.Count + " items");
            return result;
        }
    }
}

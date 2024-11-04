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
    public record GetAllDeliveryFeeQuery(bool? isLocation) : IRequest<List<DeliveryFee>>;
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
            //var query = _deliveryFeeRepository.GetQuery();
            //query = query.OrderBy(s => s.FromKm).ThenBy(s => s.Cost);
            //var result = query.ToList();
            
            var getAll = await _deliveryFeeRepository.GetAll();
            getAll = getAll.OrderBy(s => s.FromKm).ThenBy(s => s.Cost).ToList();
            if (request.isLocation != null)
            {
                if(request.isLocation == true)
                    getAll = getAll.Where(s => s.IsDistancePriceType == false).ToList();
                else
                    getAll = getAll.Where(s => s.IsDistancePriceType == true).ToList();
            }
            else { }
            _logger.LogInformation("GetAll DeliveryFees is called with total " + getAll.Count + " items");
            return getAll;
        }
    }
}

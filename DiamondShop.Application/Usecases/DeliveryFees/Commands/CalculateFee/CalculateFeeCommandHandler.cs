using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.CalculateFee
{
    internal class CalculateFeeCommandHandler : IRequestHandler<CalculateFeeCommand, Result<CalculateFeeRepsonse>>
    {
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly ILocationService _locationService;
        private readonly IDeliveryService _deliveryService;
        public CalculateFeeCommandHandler(IDeliveryFeeRepository deliveryFeeRepository, ILocationService locationService, IDeliveryService deliveryService)
        {
            _deliveryFeeRepository = deliveryFeeRepository;
            _locationService = locationService;
            _deliveryService = deliveryService;
        }

        public async Task<Result<CalculateFeeRepsonse>> Handle(CalculateFeeCommand request, CancellationToken cancellationToken)
        {
            var userLocationDetail = new LocationDetail(request.Province, request.District, request.Ward, request.Street);
            var calculateDistantResult = await _locationService.GetDistantFromBaseShopLocation(userLocationDetail, cancellationToken);
            if (calculateDistantResult.IsFailed)
                return Result.Fail(calculateDistantResult.Errors);
            decimal correctlyParsedDistantKM;
            if(calculateDistantResult.Value.DistanceUnit.ToUpper() != "KM") 
                correctlyParsedDistantKM = _locationService.ToKm(calculateDistantResult.Value.Distance);
            else
                correctlyParsedDistantKM = calculateDistantResult.Value.Distance;
            var getDeliveryFee = await _deliveryFeeRepository.GetWithDistance(correctlyParsedDistantKM, cancellationToken);
            if(getDeliveryFee == null)
                return Result.Fail("No delivery fee found for this distance");
            return new CalculateFeeRepsonse
            {
                LocationDistantData = calculateDistantResult.Value,
                DeliveryFee = getDeliveryFee
            };
        }
    }

}

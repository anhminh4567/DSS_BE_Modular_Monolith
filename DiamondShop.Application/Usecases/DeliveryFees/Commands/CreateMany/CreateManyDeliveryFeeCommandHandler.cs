using DiamondShop.Application.Dtos.Requests.Deliveries;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.CreateMany
{

    internal class CreateManyDeliveryFeeCommandHandler : IRequestHandler<CreateManyDeliveryFeeCommand, Result<List<DeliveryFee>>>
    {
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocationService _locationService;

        public CreateManyDeliveryFeeCommandHandler(IDeliveryFeeRepository deliveryFeeRepository, IUnitOfWork unitOfWork, ILocationService locationService)
        {
            _deliveryFeeRepository = deliveryFeeRepository;
            _unitOfWork = unitOfWork;
            _locationService = locationService;
        }

        public async Task<Result<List<DeliveryFee>>> Handle(CreateManyDeliveryFeeCommand request, CancellationToken cancellationToken)
        {
            var getProvince = _locationService.GetProvinces();
            var getAllDeliveryFee = await _deliveryFeeRepository.GetAll(cancellationToken);
            List<DeliveryFee> tobeAddedFees = new();
            foreach (var fee in request.fees)
            { 
                DeliveryFee newFee;
                var province = getProvince.FirstOrDefault(x => x.Id == fee.provinceId);
                if(province is null)
                {
                    return Result.Fail(new NotFoundError($"The province with name: {fee.name} is not found"));
                }
                if(getAllDeliveryFee.Any(x => x.ToLocationId == int.Parse(fee.provinceId)))
                {
                    return Result.Fail(new ConflictError($"The fee with name: {fee.name} already exists as destination founded "));
                }
                newFee = DeliveryFee.CreateLocationType(fee.name, fee.cost, province.Name, int.Parse(province.Id));
                tobeAddedFees.Add(newFee);
            }
            if (tobeAddedFees.Count == 0)
            {
                return Result.Fail(new ConflictError("No fee to be added, fill in something to add, Not a major error"));
            }
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            await _deliveryFeeRepository.CreateRange(tobeAddedFees, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken); 
            await _unitOfWork.CommitAsync(cancellationToken);
            return tobeAddedFees;
        }
    }
}

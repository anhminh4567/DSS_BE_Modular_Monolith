using DiamondShop.Application.Dtos.Requests.Deliveries;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.DeliveryFees.Commands.CreateMany;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Models.DeliveryFees.ValueObjects;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.Update
{
    public record UpdateDeliveryFeesCommand(string feeId, CreateDeliveryFeeCommand updatedObject) : IRequest<Result<DeliveryFee>>;
    internal class UpdateDeliveryFeeCommandHandler : IRequestHandler<UpdateDeliveryFeesCommand, Result<DeliveryFee>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;

        public UpdateDeliveryFeeCommandHandler(IUnitOfWork unitOfWork, IDeliveryFeeRepository deliveryFeeRepository)
        {
            _unitOfWork = unitOfWork;
            _deliveryFeeRepository = deliveryFeeRepository;
        }

        public async Task<Result<DeliveryFee>> Handle(UpdateDeliveryFeesCommand request, CancellationToken cancellationToken)
        {
            var parsedId = DeliveryFeeId.Parse(request.feeId);
            var tryGet = await _deliveryFeeRepository.GetById(parsedId);
            if (tryGet is null)
                return Result.Fail(new NotFoundError());
            //if (request.updatedObject.type == DeliveryFeeType.Distance)
            //    tryGet.ChangeFromToKm(request.updatedObject.ToDistance!.start, request.updatedObject.ToDistance!.end);
            // else if (request.updatedObject.type == DeliveryFeeType.LocationToCity)
            tryGet.ChangeFromToCity( request.updatedObject.ToLocationCity!.destinationCity);
            //else
            //   return Result.Fail(new ConflictError("Undefined type"));
            await _deliveryFeeRepository.Update(tryGet);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(tryGet);
        }
    }
}

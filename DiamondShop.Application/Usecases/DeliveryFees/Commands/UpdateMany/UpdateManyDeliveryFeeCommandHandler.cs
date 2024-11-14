using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Deliveries;
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

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.UpdateMany
{
    public record DeliveryFeeUpdateDto (string deliveryFeeId, decimal? newPrice, string? newName, bool? setActive); 
    public record UpdateManyDeliveryFeeCommand(List<DeliveryFeeUpdateDto> deliveryFeeUpdateDtos) : IRequest<Result<List<DeliveryFee>>>;
    internal class UpdateManyDeliveryFeeCommandHandler : IRequestHandler<UpdateManyDeliveryFeeCommand, Result<List<DeliveryFee>>>
    {
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly IDeliveryFeeServices _deliveryFeeServices;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateManyDeliveryFeeCommandHandler(IDeliveryFeeRepository deliveryFeeRepository, IDeliveryFeeServices deliveryFeeServices, IUnitOfWork unitOfWork)
        {
            _deliveryFeeRepository = deliveryFeeRepository;
            _deliveryFeeServices = deliveryFeeServices;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<DeliveryFee>>> Handle(UpdateManyDeliveryFeeCommand request, CancellationToken cancellationToken)
        {
            var mappedDeliveryFees = request.deliveryFeeUpdateDtos.Select(x => new { FeeId = DeliveryFeeId.Parse(x.deliveryFeeId), Price = x.newPrice , Name = x.newName, SetActive = x.setActive }).ToList();
            var getDeliveryFees = await _deliveryFeeRepository.GetRange(mappedDeliveryFees.Select(x => x.FeeId).ToList(), cancellationToken);
            await _unitOfWork.BeginTransactionAsync();
            foreach (var deliveryFee in getDeliveryFees)
            {
                var newPrice = mappedDeliveryFees.FirstOrDefault(x => x.FeeId == deliveryFee.Id);
                if (newPrice == null)
                    continue;
                if(newPrice.Price != null)
                    deliveryFee.ChangeCost(newPrice.Price.Value);
                if (newPrice.Name != null)
                    deliveryFee.ChangeName(newPrice.Name);
                if (newPrice.SetActive != null)
                    deliveryFee.SetEnable(newPrice.SetActive.Value);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return getDeliveryFees;
        }
    }
}

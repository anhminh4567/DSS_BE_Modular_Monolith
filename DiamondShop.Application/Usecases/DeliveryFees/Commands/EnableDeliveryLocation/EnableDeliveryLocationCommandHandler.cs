using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.DeliveryFees.ValueObjects;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.EnableDeliveryLocation
{
    public record SetStatusDeliveryLocation(string id, bool setActive = true) : IRequest<Result>;
    public record EnableDeliveryLocationCommand(List<SetStatusDeliveryLocation> locationStatus) : IRequest<Result>;

    internal class EnableDeliveryLocationCommandHandler : IRequestHandler<EnableDeliveryLocationCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;

        public EnableDeliveryLocationCommandHandler(IUnitOfWork unitOfWork, IDeliveryFeeRepository deliveryFeeRepository)
        {
            _unitOfWork = unitOfWork;
            _deliveryFeeRepository = deliveryFeeRepository;
        }

        public async Task<Result> Handle(EnableDeliveryLocationCommand request, CancellationToken cancellationToken)
        {
            var getAllLocation =  await _deliveryFeeRepository.GetAll();
            await _unitOfWork.BeginTransactionAsync();
            var parsedList = request.locationStatus.Select(x => new { Id = DeliveryFeeId.Parse(x.id), SetActive = x.setActive }).ToList();
            var ids = parsedList.Select(x => x.Id).ToList();
            foreach (var location in getAllLocation)
            {
                var foundedLocation = parsedList.FirstOrDefault(x => x.Id == location.Id);
                if(foundedLocation != null)
                {
                    location.SetEnable(foundedLocation.SetActive);
                    _deliveryFeeRepository.Update(location).Wait();
                }
            }
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return Result.Ok();
        }
    }
}

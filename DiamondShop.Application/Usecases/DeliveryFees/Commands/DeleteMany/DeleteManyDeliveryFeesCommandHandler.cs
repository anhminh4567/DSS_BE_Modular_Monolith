using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Models.DeliveryFees.ValueObjects;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.DeleteMany
{
    public record DeleteManyDeliveryFeesCommand(string[] deliveryFeeIds) : IRequest<Result>;
    internal class DeleteManyDeliveryFeesCommandHandler : IRequestHandler<DeleteManyDeliveryFeesCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;

        public DeleteManyDeliveryFeesCommandHandler(IUnitOfWork unitOfWork, IDeliveryFeeRepository deliveryFeeRepository)
        {
            _unitOfWork = unitOfWork;
            _deliveryFeeRepository = deliveryFeeRepository;
        }

        public async Task<Result> Handle(DeleteManyDeliveryFeesCommand request, CancellationToken cancellationToken)
        {
            DeliveryFeeId[] idParsed = request.deliveryFeeIds.Select(DeliveryFeeId.Parse).ToArray();
            await _deliveryFeeRepository.ExecuteDeleteRanges(idParsed);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}

using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Jewelries.Commands;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Staff
{
    public record ProceedRequestingRequestCommand(CustomizeRequest CustomizeRequest) : IRequest<Result<CustomizeRequest>>;
    internal class ProceedRequestingRequestCommandHandler : IRequestHandler<ProceedRequestingRequestCommand, Result<CustomizeRequest>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISender _sender;
        public ProceedRequestingRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork, ISender sender)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
            _sender = sender;
        }

        public async Task<Result<CustomizeRequest>> Handle(ProceedRequestingRequestCommand request, CancellationToken token)
        {

            request.Deconstruct(out CustomizeRequest customizeRequest);
            await _unitOfWork.BeginTransactionAsync(token);
            if (customizeRequest.Status != CustomizeRequestStatus.Requesting)
                return Result.Fail("This request can't be accepted anymore");
            //Create Jewelry
            var diamondList = customizeRequest.DiamondRequests.Count > 0 ? customizeRequest.DiamondRequests.Select(p => p.DiamondId.Value).ToList() : null;
            JewelryRequestDto jewelryRequestDto = new(customizeRequest.JewelryModelId.Value, customizeRequest.SizeId.Value, customizeRequest.MetalId.Value, ProductStatus.Locked);
            var jewelryResult = await _sender.Send(new CreateJewelryCommand(jewelryRequestDto, customizeRequest.SideDiamondId?.Value, diamondList));
            if(jewelryResult.IsFailed)
                return Result.Fail(jewelryResult.Errors);
            customizeRequest.JewelryId = jewelryResult.Value.Id;
            customizeRequest.Status = CustomizeRequestStatus.Accepted;
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return customizeRequest;
        }
    }
}

using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Staff;
using DiamondShop.Application.Usecases.Jewelries.Commands.Create;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ErrorMessages;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Customer
{
    public record CustomerAcceptRequestCommand(string CustomizeRequestId, string AccountId) : IRequest<Result<CustomizeRequest>>;
    internal class CustomerAcceptRequestCommandHandler : IRequestHandler<CustomerAcceptRequestCommand, Result<CustomizeRequest>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISender _sender;
        private readonly ICustomizeRequestService _customizeRequestService;
        public CustomerAcceptRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork, ISender sender, ICustomizeRequestService customizeRequestService)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
            _sender = sender;
            _customizeRequestService = customizeRequestService;
        }

        public async Task<Result<CustomizeRequest>> Handle(CustomerAcceptRequestCommand request, CancellationToken token)
        {

            request.Deconstruct(out string customizeRequestId, out string accountId);
            await _unitOfWork.BeginTransactionAsync(token);
            var customizeRequest = await _customizeRequestRepository.GetById(CustomizeRequestId.Parse(customizeRequestId));
            if (customizeRequest == null)
                return Result.Fail(CustomizeRequestErrors.CustomizeRequestNotFoundError);
            if (customizeRequest.ExpiredDate < DateTime.UtcNow)
            {
                return Result.Fail(CustomizeRequestErrors.ExpiredError);
            }
            if (customizeRequest.AccountId.Value != accountId)
                return Result.Fail(CustomizeRequestErrors.NoPermissionError);
            if (customizeRequest.Status != CustomizeRequestStatus.Priced)
                return Result.Fail(CustomizeRequestErrors.UnrequestableError);
            var diamondList = customizeRequest.DiamondRequests.Count > 0 ? customizeRequest.DiamondRequests.Select(p => p.DiamondId.Value).ToList() : null;
            JewelryRequestDto jewelryRequestDto = new(customizeRequest.JewelryModelId.Value, customizeRequest.SizeId.Value, customizeRequest.MetalId.Value, ProductStatus.PreOrder);
            var jewelryResult = await _sender.Send(new CreateJewelryCommand(jewelryRequestDto, customizeRequest.SideDiamondId?.Value, diamondList));
            if (jewelryResult.IsFailed)
                return Result.Fail(jewelryResult.Errors);
            customizeRequest.JewelryId = jewelryResult.Value.Id;
            customizeRequest.SetAccepted();
            customizeRequest.ResetExpiredDate();
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return customizeRequest;
        }
    }
}

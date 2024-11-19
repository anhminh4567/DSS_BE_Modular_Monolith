using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Staff;
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
    public record CustomerRequestingRequestCommand(string CustomizeRequestId, string AccountId) : IRequest<Result<CustomizeRequest>>;
    internal class CustomerRequestingRequestCommandHandler : IRequestHandler<CustomerRequestingRequestCommand, Result<CustomizeRequest>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISender _sender;
        private readonly ICustomizeRequestService _customizeRequestService;
        public CustomerRequestingRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork, ISender sender, ICustomizeRequestService customizeRequestService)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
            _sender = sender;
            _customizeRequestService = customizeRequestService;
        }

        public async Task<Result<CustomizeRequest>> Handle(CustomerRequestingRequestCommand request, CancellationToken token)
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
            customizeRequest.Status = CustomizeRequestStatus.Requesting;
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            //staff auto accept
            var staffAccepted = new StaffProceedCustomizeRequestCommand(customizeRequest.Id.Value,null, null);
            var result = await _sender.Send(staffAccepted);
            if (result.IsFailed)
            {
                await _unitOfWork.RollBackAsync(token);
                return Result.Fail(result.Errors);
            }
            //staff auto accept finish
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            _customizeRequestService.SetStage(customizeRequest);
            return customizeRequest;
        }
    }
}

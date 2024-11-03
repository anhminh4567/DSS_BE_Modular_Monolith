using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Reject
{
    public record CustomerRejectRequestCommand(string CustomizeRequestId, string AccountId) : IRequest<Result<CustomizeRequest>>;
    internal class CustomerRejectRequestCommandHandler : IRequestHandler<CustomerRejectRequestCommand, Result<CustomizeRequest>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerRejectRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CustomizeRequest>> Handle(CustomerRejectRequestCommand request, CancellationToken token)
        {

            request.Deconstruct(out string customizeRequestId, out string accountId);
            await _unitOfWork.BeginTransactionAsync(token);
            var customizeRequest = await _customizeRequestRepository.GetById(CustomizeRequestId.Parse(customizeRequestId));
            if (customizeRequest == null)
                return Result.Fail("This request doens't exist");
            if (customizeRequest.AccountId.Value != accountId)
                return Result.Fail("Only the owner of this request can reject it");
            if (customizeRequest.Status != CustomizeRequestStatus.Priced)
                return Result.Fail("You can't reject this request anymore");
            customizeRequest.Status = CustomizeRequestStatus.Customer_Rejected;
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return customizeRequest;
        }
    }
}

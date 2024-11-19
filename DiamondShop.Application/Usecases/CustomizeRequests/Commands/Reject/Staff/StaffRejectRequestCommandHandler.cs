using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Reject.Staff
{
    public record StaffRejectRequestCommand(string CustomizeRequestId) : IRequest<Result<CustomizeRequest>>;
    internal class StaffRejectRequestCommandHandler : IRequestHandler<StaffRejectRequestCommand, Result<CustomizeRequest>>
    {
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomizeRequestService _customizeRequestService;

        public StaffRejectRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, ICustomizeRequestService customizeRequestService)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _customizeRequestService = customizeRequestService;
        }

        public async Task<Result<CustomizeRequest>> Handle(StaffRejectRequestCommand request, CancellationToken token)
        {
            request.Deconstruct(out string customizeRequestId);
            await _unitOfWork.BeginTransactionAsync(token);
            var customizeRequest = await _customizeRequestRepository.GetById(CustomizeRequestId.Parse(customizeRequestId));
            if (customizeRequest == null)
                return Result.Fail("This request doens't exist");
            if (customizeRequest.Status != CustomizeRequestStatus.Pending && customizeRequest.Status != CustomizeRequestStatus.Requesting)
                return Result.Fail("You can't reject this request anymore");
            customizeRequest.Status = CustomizeRequestStatus.Shop_Rejected;
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            var isPriced = customizeRequest.Status == CustomizeRequestStatus.Requesting;
            if (customizeRequest.DiamondRequests.Count() > 0)
            {
                foreach (var diamondReq in customizeRequest.DiamondRequests)
                {
                    if (diamondReq.Diamond != null)
                    {
                        if (diamondReq.Diamond.Status == ProductStatus.Active)
                        {
                            diamondReq.Diamond.SetSell();
                            await _diamondRepository.Update(diamondReq.Diamond);
                        }
                        else if (diamondReq.Diamond.Status == ProductStatus.PreOrder)
                        {
                            await _diamondRepository.Delete(diamondReq.Diamond);
                        }
                        await _unitOfWork.SaveChangesAsync(token);
                    }
                }
            }
            await _unitOfWork.CommitAsync(token);
            _customizeRequestService.SetStage(customizeRequest, isPriced);
            return customizeRequest;
        }
    }
}

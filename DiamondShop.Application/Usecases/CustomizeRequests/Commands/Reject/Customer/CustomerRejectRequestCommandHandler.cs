using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Reject.Customer
{
    public record CustomerRejectRequestCommand(string CustomizeRequestId, string AccountId) : IRequest<Result<CustomizeRequest>>;
    internal class CustomerRejectRequestCommandHandler : IRequestHandler<CustomerRejectRequestCommand, Result<CustomizeRequest>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomizeRequestService _customizeRequestService;

        public CustomerRejectRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork, IJewelryRepository jewelryRepository, IDiamondRepository diamondRepository, ICustomizeRequestService customizeRequestService)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
            _jewelryRepository = jewelryRepository;
            _diamondRepository = diamondRepository;
            _customizeRequestService = customizeRequestService;
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
            if (customizeRequest.Status != CustomizeRequestStatus.Priced && customizeRequest.Status != CustomizeRequestStatus.Accepted)
                return Result.Fail("You can't reject this request anymore");
            if (customizeRequest.Status == CustomizeRequestStatus.Accepted && customizeRequest.JewelryId != null)
            {
                var jewelry = await _jewelryRepository.GetById(customizeRequest.JewelryId);
                if (jewelry == null)
                    return Result.Fail("Can't get requested jewelry");
                jewelry.Status = ProductStatus.Active;
                await _jewelryRepository.Update(jewelry);
                await _unitOfWork.SaveChangesAsync(token);
            }
            customizeRequest.Status = CustomizeRequestStatus.Customer_Rejected;
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            if (customizeRequest.DiamondRequests.Count() > 0)
            {
                foreach (var diamondReq in customizeRequest.DiamondRequests)
                {
                    if (diamondReq.Diamond != null)
                    {
                        if(diamondReq.Diamond.Status == ProductStatus.Active)
                        {
                            diamondReq.Diamond.SetSell();
                            await _diamondRepository.Update(diamondReq.Diamond);
                        }
                        else if(diamondReq.Diamond.Status == ProductStatus.PreOrder)
                        {
                            await _diamondRepository.Delete(diamondReq.Diamond);
                        }                        
                        await _unitOfWork.SaveChangesAsync(token);
                    }
                }
            }
            await _unitOfWork.CommitAsync(token);
            _customizeRequestService.SetStage(customizeRequest);
            return customizeRequest;
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ErrorMessages;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Cancel
{
    public record CustomerCancelRequestCommand(string CustomizeRequestId, string AccountId) : IRequest<Result<CustomizeRequest>>;
    internal class CustomerCancelRequestCommandHandler : IRequestHandler<CustomerCancelRequestCommand, Result<CustomizeRequest>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomizeRequestService _customizeRequestService;

        public CustomerCancelRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, IUnitOfWork unitOfWork, IJewelryRepository jewelryRepository, IDiamondRepository diamondRepository, ICustomizeRequestService customizeRequestService)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _unitOfWork = unitOfWork;
            _jewelryRepository = jewelryRepository;
            _diamondRepository = diamondRepository;
            _customizeRequestService = customizeRequestService;
        }

        public async Task<Result<CustomizeRequest>> Handle(CustomerCancelRequestCommand request, CancellationToken token)
        {

            await _unitOfWork.BeginTransactionAsync(token);
            var customizeRequest = await _customizeRequestRepository.GetById(CustomizeRequestId.Parse(customizeRequestId));
            if (customizeRequest == null)
                return Result.Fail(CustomizeRequestErrors.CustomizeRequestNotFoundError);
            if (customizeRequest.AccountId.Value != accountId)
                return Result.Fail(CustomizeRequestErrors.NoPermissionError);
            if (customizeRequest.Status != CustomizeRequestStatus.Pending && customizeRequest.Status != CustomizeRequestStatus.Requesting && customizeRequest.Status != CustomizeRequestStatus.Accepted)
                return Result.Fail(CustomizeRequestErrors.UncancelableError);
            if (customizeRequest.Status == CustomizeRequestStatus.Accepted)
            {
                if (customizeRequest.JewelryId != null)
                {
                    //Jewelry is preorder so delete it
                    var jewelry = await _jewelryRepository.GetById(customizeRequest.JewelryId);
                    if (jewelry == null)
                        return Result.Fail(JewelryErrors.JewelryNotFoundError);
                    await _jewelryRepository.Delete(jewelry);
                    await _unitOfWork.SaveChangesAsync(token);
                }
                else
                    return Result.Fail(JewelryErrors.JewelryNotFoundError);
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
                        else
                            return Result.Fail(DiamondErrors.DiamondNotFoundError);
                    }
                }
            }
            customizeRequest.SetCustomerCancel();
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return customizeRequest;
        }
    }
}

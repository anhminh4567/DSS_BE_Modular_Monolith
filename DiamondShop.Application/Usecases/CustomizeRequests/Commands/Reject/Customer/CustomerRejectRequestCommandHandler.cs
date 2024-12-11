using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ErrorMessages;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
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
                return Result.Fail(CustomizeRequestErrors.CustomizeRequestNotFoundError);
            if (customizeRequest.AccountId.Value != accountId)
                return Result.Fail(CustomizeRequestErrors.NoPermissionError);
            if (customizeRequest.Status != CustomizeRequestStatus.Priced)
                return Result.Fail(CustomizeRequestErrors.UnrejectableError);
            if (customizeRequest.DiamondRequests.Count() > 0)
            {
                List<Diamond> tobeSellDiamonds = new();
                List<Diamond> tobeDeleteDiamonds = new();
                foreach (var diamondReq in customizeRequest.DiamondRequests)
                {
                    if (diamondReq.Diamond != null)
                    {
                        if(diamondReq.Diamond.Status == ProductStatus.Locked)
                        {
                            diamondReq.DiamondId = null;
                            tobeSellDiamonds.Add(diamondReq.Diamond);
                        }
                        else if(diamondReq.Diamond.Status == ProductStatus.PreOrder)
                        {
                            diamondReq.DiamondId = null;
                            tobeDeleteDiamonds.Add(diamondReq.Diamond);
                        }                        
                    }
                }
                tobeSellDiamonds.ForEach(x => x.SetSell());
                _customizeRequestRepository.Update(customizeRequest).Wait();
                _diamondRepository.UpdateRange(tobeSellDiamonds);
                tobeDeleteDiamonds.ForEach(x => _diamondRepository.Delete(x).Wait());
                await _unitOfWork.SaveChangesAsync(token);

            }
            customizeRequest.SetCustomerReject();
            await _customizeRequestRepository.Update(customizeRequest);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return customizeRequest;
        }
    }
}

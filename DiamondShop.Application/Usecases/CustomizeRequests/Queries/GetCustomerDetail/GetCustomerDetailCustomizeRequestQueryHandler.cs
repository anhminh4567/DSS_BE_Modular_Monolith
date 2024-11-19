using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ErrorMessages;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetCustomerDetail
{
    public record GetCustomerDetailCustomizeRequestQuery(string RequestId, string AccountId) : IRequest<Result<CustomizeRequest>>;
    internal class GetCustomerDetailCustomizeRequestQueryHandler : IRequestHandler<GetCustomerDetailCustomizeRequestQuery, Result<CustomizeRequest>>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IJewelryService _jewelryService;
        private readonly IJewelryModelService _jewelryModelService;
        private readonly ICustomizeRequestService _customizeRequestService;
        private readonly IDiamondServices _diamondServices;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        public GetCustomerDetailCustomizeRequestQueryHandler(ICustomizeRequestRepository customizeRequestRepository, IJewelryModelService jewelryModelService, IJewelryService jewelryService, IDiscountRepository discountRepository, ICustomizeRequestService customizeRequestService, IDiamondPriceRepository diamondPriceRepository, IDiamondServices diamondServices)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _jewelryModelService = jewelryModelService;
            _jewelryService = jewelryService;
            _discountRepository = discountRepository;
            _customizeRequestService = customizeRequestService;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondServices = diamondServices;
        }

        public async Task<Result<CustomizeRequest>> Handle(GetCustomerDetailCustomizeRequestQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string requestId, out string accountId);
            var discounts = await _discountRepository.GetActiveDiscount();
            var customizeRequest = await _customizeRequestRepository.GetDetail(CustomizeRequestId.Parse(requestId));
            if (customizeRequest == null)
                return Result.Fail(CustomizeRequestErrors.CustomizeRequestNotFoundError);
            if (customizeRequest.AccountId != AccountId.Parse(accountId))
                return Result.Fail(CustomizeRequestErrors.NoPermissionError);
            var model = customizeRequest.JewelryModel;
            if (model == null)
                return Result.Fail(JewelryModelErrors.JewelryModelNotFoundError);
            var sizeMetal = customizeRequest.JewelryModel.SizeMetals.FirstOrDefault(p => p.SizeId == customizeRequest.SizeId && p.MetalId == customizeRequest.MetalId);
            await _jewelryModelService.AddSettingPrice(model, sizeMetal, customizeRequest.SideDiamond);
            bool isPriced = false;
            if (customizeRequest.SideDiamond != null && customizeRequest.SideDiamond.TotalPrice != 0)
                isPriced = true;
            await _jewelryModelService.AssignJewelryModelDiscount(model, discounts);
            var diamondRequests = customizeRequest.DiamondRequests;
            foreach (var diamondRequest in diamondRequests)
            {
                var diamond = diamondRequest.Diamond;
                if (diamond == null)
                {
                    isPriced = false;
                    break;
                }
                var prices = await _diamondPriceRepository.GetPrice(diamond.Cut, diamond.DiamondShape, false, cancellationToken);
                var diamondPrice = await _diamondServices.GetDiamondPrice(diamond, prices);
                if (diamondPrice.Price != 0)
                {
                    isPriced = isPriced && true;
                }
            }
            if (customizeRequest.Status == CustomizeRequestStatus.Accepted)
            {
                var jewelry = customizeRequest.Jewelry;
                if (jewelry == null)
                    return Result.Fail(JewelryErrors.JewelryNotFoundError);
                if (sizeMetal == null)
                    return Result.Fail(JewelryModelErrors.SizeMetal.SizeMetalNotFoundError);
                _jewelryService.AddPrice(jewelry, sizeMetal);
                await _jewelryService.AssignJewelryDiscount(jewelry, discounts);
            }
            _customizeRequestService.SetStage(customizeRequest, isPriced);
            return customizeRequest;
        }
    }
}

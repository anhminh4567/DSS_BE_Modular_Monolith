using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ErrorMessages;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetDetail
{
    public record GetDetailCustomizeRequestQuery(string RequestId) : IRequest<Result<CustomizeRequest>>;
    internal class GetDetailCustomizeRequestQueryHandler : IRequestHandler<GetDetailCustomizeRequestQuery, Result<CustomizeRequest>>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IJewelryService _jewelryService;
        private readonly IJewelryModelService _jewelryModelService;
        private readonly IDiamondServices _diamondServices;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly ICustomizeRequestService _customizeRequestService;
        public GetDetailCustomizeRequestQueryHandler(ICustomizeRequestRepository customizeRequestRepository, IJewelryModelService jewelryModelService, IJewelryService jewelryService, IDiscountRepository discountRepository, IDiamondServices diamondServices, IDiamondPriceRepository diamondPriceRepository, ICustomizeRequestService customizeRequestService)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _jewelryModelService = jewelryModelService;
            _jewelryService = jewelryService;
            _discountRepository = discountRepository;
            _diamondServices = diamondServices;
            _diamondPriceRepository = diamondPriceRepository;
            _customizeRequestService = customizeRequestService;
        }

        public async Task<Result<CustomizeRequest>> Handle(GetDetailCustomizeRequestQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string requestId);
            var discounts = await _discountRepository.GetActiveDiscount();
            var customizeRequest = await _customizeRequestRepository.GetDetail(CustomizeRequestId.Parse(requestId));
            if (customizeRequest == null)
                return Result.Fail(CustomizeRequestErrors.CustomizeRequestNotFoundError);
            var model = customizeRequest.JewelryModel;
            if (model == null)
                return Result.Fail(CustomizeRequestErrors.NoPermissionError);
            var sizeMetal = customizeRequest.JewelryModel.SizeMetals.FirstOrDefault(p => p.SizeId == customizeRequest.SizeId && p.MetalId == customizeRequest.MetalId);
            await _jewelryModelService.AddSettingPrice(model, sizeMetal, customizeRequest.SideDiamond);
            await _jewelryModelService.AssignJewelryModelDiscount(model, discounts);
            var diamondRequests = customizeRequest.DiamondRequests;
            int pos = 1;
            foreach (var diamondRequest in diamondRequests)
            {
                diamondRequest.Position = pos++;
                var diamond = diamondRequest.Diamond;
                if (diamond != null)
                {
                    var prices = await _diamondPriceRepository.GetPrice(diamond.Cut, diamond.DiamondShape, diamond.IsLabDiamond, cancellationToken);
                    var diamondPrice = await _diamondServices.GetDiamondPrice(diamond, prices);
                }
            }
            if (customizeRequest.Status == CustomizeRequestStatus.Accepted)
            {
                var jewelry = customizeRequest.Jewelry;
                if(customizeRequest.Order != null && customizeRequest.JewelryId == null)
                    return customizeRequest;
                if (jewelry == null)
                    return Result.Fail(JewelryErrors.JewelryNotFoundError);
                if (sizeMetal == null)
                    return Result.Fail(JewelryModelErrors.SizeMetal.SizeMetalNotFoundError);
                _jewelryService.AddPrice(jewelry, sizeMetal);
                await _jewelryService.AssignJewelryDiscount(jewelry, discounts);
            }
            return customizeRequest;
        }

    }
}

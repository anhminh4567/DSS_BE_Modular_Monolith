using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetDetail
{
    public record GetDetailCustomizeRequestQuery(string requestId) : IRequest<Result<CustomizeRequest>>;
    internal class GetDetailCustomizeRequestQueryHandler : IRequestHandler<GetDetailCustomizeRequestQuery, Result<CustomizeRequest>>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IJewelryService _jewelryService;
        private readonly IJewelryModelService _jewelryModelService;
        public GetDetailCustomizeRequestQueryHandler(ICustomizeRequestRepository customizeRequestRepository, IJewelryModelService jewelryModelService, IJewelryService jewelryService, IDiscountRepository discountRepository)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _jewelryModelService = jewelryModelService;
            _jewelryService = jewelryService;
            _discountRepository = discountRepository;
        }

        public async Task<Result<CustomizeRequest>> Handle(GetDetailCustomizeRequestQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string requestId);
            var discounts = await _discountRepository.GetActiveDiscount();
            var customizeRequest = await _customizeRequestRepository.GetDetail(CustomizeRequestId.Parse(requestId));
            var model = customizeRequest.JewelryModel;
            if (model == null)
                return Result.Fail("Can't get the requested jewelry model");
            var sizeMetal = customizeRequest.JewelryModel.SizeMetals.FirstOrDefault(p => p.SizeId == customizeRequest.SizeId && p.MetalId == customizeRequest.MetalId);
            if (customizeRequest.Status == CustomizeRequestStatus.Accepted)
            {
                var jewelry = customizeRequest.Jewelry;
                if (jewelry == null)
                    return Result.Fail("Can't get the requested jewelry");
                if (sizeMetal == null)
                    return Result.Fail("Can't get size and metal option for the requested jewelry");
                _jewelryService.AddPrice(jewelry, sizeMetal);
                await _jewelryService.AssignJewelryDiscount(jewelry, discounts);
            }
            if (customizeRequest.Status == CustomizeRequestStatus.Priced || customizeRequest.Status == CustomizeRequestStatus.Requesting)
            {
                await _jewelryModelService.AddSettingPrice(model, sizeMetal, customizeRequest.SideDiamond);
                await _jewelryModelService.AssignJewelryModelDiscount(model, discounts);
            }
            if (customizeRequest == null)
                return Result.Fail("This customize request doesn't exist");
            return customizeRequest;
        }
    }
}

using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetDetail
{
    public record GetDiamondDetail(string diamondId ) : IRequest<Result<Diamond>>;
    internal class GetDiamondDetailQueryHandler : IRequestHandler<GetDiamondDetail, Result<Diamond>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly ILogger<GetDiamondDetail> _logger;
        private readonly IDiamondServices _diamondServices;
        private readonly IDiscountService _discountService;
        private readonly IDiscountRepository _discountRepository;
        private readonly IAccountRepository _accountRepository;

        public GetDiamondDetailQueryHandler(IDiamondRepository diamondRepository, IDiamondPriceRepository diamondPriceRepository, ILogger<GetDiamondDetail> logger, IDiamondServices diamondServices, IDiscountService discountService, IDiscountRepository discountRepository, IAccountRepository accountRepository)
        {
            _diamondRepository = diamondRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _logger = logger;
            _diamondServices = diamondServices;
            _discountService = discountService;
            _discountRepository = discountRepository;
            _accountRepository = accountRepository;
        }

        public async Task<Result<Diamond>> Handle(GetDiamondDetail request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("called from getDiamondDetail");
            var parsedId = DiamondId.Parse(request.diamondId);
            var getResult = await _diamondRepository.GetById(parsedId);
            if (getResult == null)
            {
                return Result.Fail(new NotFoundError());
            }
            bool isFancyShape = DiamondShape.IsFancyShape(getResult.DiamondShapeId);
            var prices = await _diamondPriceRepository.GetPrice(getResult.Cut.Value,getResult.DiamondShape, getResult.IsLabDiamond,cancellationToken);
            var diamondPrice = await _diamondServices.GetDiamondPrice(getResult, prices);
            //getResult.DiamondPrice = diamondPrice;
            //var testingOnly = await _diamondRepository.GetByIdIncludeDiscountAndPromotion(parsedId);
            var getAllDiscount = await _discountRepository.GetActiveDiscount();
            _diamondServices.AssignDiamondDiscount(getResult, getAllDiscount).Wait();
            if(getResult.ProductLock != null && getResult.Status == Domain.Common.Enums.ProductStatus.LockForUser)
            {
                if(getResult.ProductLock.AccountId != null)
                {
                    var account = await _accountRepository.GetById(getResult.ProductLock.AccountId);
                    getResult.ProductLock.Account = account;
                }
            }
            return Result.Ok(getResult);
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.ChangePriceOffset
{
    public record ChangeDiamondPriceOffsetRequest(decimal? priceOffset, decimal? extraFee);
    public record ChangeDiamondPriceOffsetCommannd(string diamondId, decimal? priceOffset, decimal? extraFee) : IRequest<Result<Diamond>>;
    internal class ChangeDiamondPriceOffsetCommanndHandler : IRequestHandler<ChangeDiamondPriceOffsetCommannd, Result<Diamond>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public ChangeDiamondPriceOffsetCommanndHandler(IDiamondRepository diamondRepository, IUnitOfWork unitOfWork, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _diamondRepository = diamondRepository;
            _unitOfWork = unitOfWork;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<Diamond>> Handle(ChangeDiamondPriceOffsetCommannd request, CancellationToken cancellationToken)
        {
            var diamondId = DiamondId.Parse(request.diamondId);
            var getDiamond = await _diamondRepository.GetById(diamondId);
            if(getDiamond == null)
            {
                return Result.Fail(DiamondErrors.DiamondNotFoundError);
            }
            var diamondRule = _optionsMonitor.CurrentValue.DiamondRule;
            if(request.priceOffset > diamondRule.MaxPriceOffset || request.priceOffset < diamondRule.MinPriceOffset)
            {
                return Result.Fail(DiamondErrors.UpdateError.PriceOffsetNotInLimit);
            }
            if (getDiamond.Status == Domain.Common.Enums.ProductStatus.Sold)
                return Result.Fail(DiamondErrors.SoldError());
            if(request.priceOffset != null)
                getDiamond.ChangeOffset(request.priceOffset.Value);
            if (request.extraFee != null)
            {
                var fee = MoneyVndRoundUpRules.RoundAmountFromDecimal(request.extraFee.Value);
                getDiamond.SetExtraFee(fee);
            }
            else
            {
                getDiamond.SetExtraFee(null);
            }

            await _diamondRepository.Update(getDiamond);
            await _unitOfWork.SaveChangesAsync();
            return getDiamond;
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
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
    public record ChangeDiamondPriceOffsetCommannd(string diamondId, decimal priceOffset) : IRequest<Result<Diamond>>;
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
            getDiamond.ChangeOffset(request.priceOffset);
            await _diamondRepository.Update(getDiamond);
            await _unitOfWork.SaveChangesAsync();
            return getDiamond;
        }
    }
}

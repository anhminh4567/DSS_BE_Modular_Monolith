using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ErrorMessages;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.LockPriceForJewelryDiamonds
{
    public record LockPriceForJewelryCommand(string jewelryId, string diamondId,bool isUnlock, int lockHour, decimal? LockedPriceForCustomer) : IRequest<Result<Diamond>>;
    internal class LockPriceForJewelryCommandHandler : IRequestHandler<LockPriceForJewelryCommand, Result<Diamond>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public LockPriceForJewelryCommandHandler(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, IAccountRepository accountRepository, IJewelryRepository jewelryRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _accountRepository = accountRepository;
            _jewelryRepository = jewelryRepository;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<Diamond>> Handle(LockPriceForJewelryCommand request, CancellationToken cancellationToken)
        {
            var parsedDiamondId = DiamondId.Parse(request.diamondId);
            var parsedJewelryId = JewelryId.Parse(request.jewelryId);

            var getJewellery = await _jewelryRepository.GetById(parsedJewelryId);
            if (getJewellery is null)
                return Result.Fail(JewelryErrors.JewelryNotFoundError);
            if (getJewellery.Status == Domain.Common.Enums.ProductStatus.Sold)
                return Result.Fail(JewelryErrors.IsSold);
            var diamond = getJewellery.Diamonds.FirstOrDefault(x => x.Id == parsedDiamondId);
            if (diamond is null)
                return Result.Fail(DiamondErrors.DiamondNotFoundError);


            if (request.isUnlock)
                diamond.RemoveLock();

            else
                diamond.SetLockPriceForJewelry(request.lockHour, request.LockedPriceForCustomer, _optionsMonitor.CurrentValue.DiamondRule);
            await _diamondRepository.Update(diamond);
            await _unitOfWork.SaveChangesAsync();
            return diamond;
            throw new NotImplementedException();
        }
    }
}

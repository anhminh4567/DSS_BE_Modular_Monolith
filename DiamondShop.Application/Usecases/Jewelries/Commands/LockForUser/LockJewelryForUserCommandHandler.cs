using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ErrorMessages;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Commands.LockForUser
{
    public record LockJewelryForUserCommand(string jewelryId, string? customerId, bool isUnlock, int lockHour, decimal? LockedPriceForCustomer) : IRequest<Result<Jewelry>>;
    internal class LockJewelryForUserCommandHandler : IRequestHandler<LockJewelryForUserCommand, Result<Jewelry>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IJewelryService _jewelryService;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public LockJewelryForUserCommandHandler(IUnitOfWork unitOfWork, IJewelryRepository jewelryRepository, IAccountRepository accountRepository, IJewelryService jewelryService, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _unitOfWork = unitOfWork;
            _jewelryRepository = jewelryRepository;
            _accountRepository = accountRepository;
            _jewelryService = jewelryService;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<Jewelry>> Handle(LockJewelryForUserCommand request, CancellationToken cancellationToken)
        {
            var parsedJewelryId = JewelryId.Parse(request.jewelryId);
            Account? customer = null;
            if(request.customerId != null)
            {
                var parsedCustomerId = AccountId.Parse(request.customerId);
                customer = await _accountRepository.GetById(parsedCustomerId);
                if (customer is null)
                    return Result.Fail(AccountErrors.AccountNotFoundError);
            }

            var jewelry = await _jewelryRepository.GetById(parsedJewelryId);
            if (jewelry is null)
                return Result.Fail(JewelryErrors.JewelryNotFoundError);
            var checkLockStatus = jewelry.CanBeLock();
            if(checkLockStatus.IsFailed)
                return checkLockStatus;
            jewelry.SetLockForUser(customer,request.lockHour,request.isUnlock, _optionsMonitor.CurrentValue.JewelryRules);
            await _jewelryRepository.Update(jewelry);
            await _unitOfWork.SaveChangesAsync();
            return jewelry;
            throw new NotImplementedException();
        }
    }
}

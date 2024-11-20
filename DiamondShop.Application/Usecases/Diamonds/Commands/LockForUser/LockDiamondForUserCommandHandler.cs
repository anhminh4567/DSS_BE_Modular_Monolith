using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ErrorMessages;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.LockForUser
{
    public record LockDiamondForUserCommand(bool isUnlock,string customerId, string diamondId, int lockHour, decimal? LockedPriceForCustomer) : IRequest<Result<Diamond>>;
    internal class LockDiamondForUserCommandHandler : IRequestHandler<LockDiamondForUserCommand, Result<Diamond>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAuthenticationService _authenticationService;

        public LockDiamondForUserCommandHandler(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, IAccountRepository accountRepository, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _accountRepository = accountRepository;
            _authenticationService = authenticationService;
        }

        public async Task<Result<Diamond>> Handle(LockDiamondForUserCommand request, CancellationToken cancellationToken)
        {
            var parsedCustomerId = AccountId.Parse(request.customerId);
            var parsedDiamondId = DiamondId.Parse(request.diamondId);
            var getCustomer = await _accountRepository.GetById(parsedCustomerId);
            if(getCustomer is null)
                return Result.Fail(AccountErrors.AccountNotFoundError);
            
            var getDiamond = await _diamondRepository.GetById(parsedDiamondId);
            if (getDiamond is null)
                return Result.Fail(DiamondErrors.DiamondNotFoundError);

            if (getDiamond.Status == Domain.Common.Enums.ProductStatus.Sold)
                return Result.Fail(DiamondErrors.LockError.SoldError);

            if (request.isUnlock)
                getDiamond.SetSell();
            
            else
                getDiamond.SetLockForUser(getCustomer, request.lockHour,request.LockedPriceForCustomer);
            
            await _unitOfWork.SaveChangesAsync();
            return getDiamond;
        }
    }
}

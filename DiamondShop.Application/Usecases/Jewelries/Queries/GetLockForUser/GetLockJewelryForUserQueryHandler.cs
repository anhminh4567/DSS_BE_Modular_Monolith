using DiamondShop.Domain.Models.AccountAggregate.ErrorMessages;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetLockForUser
{
    public record GetLockJewelryForUserQuery(string accountId) : IRequest<List<Jewelry>>;
    internal class GetLockJewelryForUserQueryHandler : IRequestHandler<GetLockJewelryForUserQuery, List<Jewelry>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryService _jewelryService;
        private readonly IAccountRepository _accountRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;

        public GetLockJewelryForUserQueryHandler(IJewelryRepository jewelryRepository, IJewelryService jewelryService, IAccountRepository accountRepository, IDiscountRepository discountRepository, ISizeMetalRepository sizeMetalRepository)
        {
            _jewelryRepository = jewelryRepository;
            _jewelryService = jewelryService;
            _accountRepository = accountRepository;
            _discountRepository = discountRepository;
            _sizeMetalRepository = sizeMetalRepository;
        }

        public async Task<List<Jewelry>> Handle(GetLockJewelryForUserQuery request, CancellationToken cancellationToken)
        {
            var accountId = AccountId.Parse(request.accountId);
            var account = await _accountRepository.GetById(accountId, cancellationToken);
            if (account == null)
                throw new Exception(AccountErrors.AccountNotFoundError.Message);
            List<Diamond> diamonds = new();
            var lockJewelry = await _jewelryRepository.GetLockJewelryForUser(account, cancellationToken);
            var getDiscounts = await _discountRepository.GetActiveDiscount();
            foreach (var jewelry in lockJewelry)
            {
                _jewelryService.AddPrice(jewelry, _sizeMetalRepository);
                await _jewelryService.AssignJewelryDiscount(jewelry,getDiscounts);
            }
            return lockJewelry;
            throw new NotImplementedException();
        }
    }
}

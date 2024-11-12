using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetLockItemsForUser
{
    public record GetLockDiamondsForUserQuery(string accountId) : IRequest<List<Diamond>>;
    internal class GetLockDiamondsForUserQueryHandler : IRequestHandler<GetLockDiamondsForUserQuery, List<Diamond>>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IDiamondServices _diamondServices;
        private readonly IAccountRepository _accountRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;

        public GetLockDiamondsForUserQueryHandler(IDiscountRepository discountRepository, IDiamondRepository diamondRepository, IPromotionRepository promotionRepository, IDiamondServices diamondServices, IAccountRepository accountRepository, IDiamondPriceRepository diamondPriceRepository, IDiamondShapeRepository diamondShapeRepository)
        {
            _discountRepository = discountRepository;
            _diamondRepository = diamondRepository;
            _promotionRepository = promotionRepository;
            _diamondServices = diamondServices;
            _accountRepository = accountRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondShapeRepository = diamondShapeRepository;
        }

        public async Task<List<Diamond>> Handle(GetLockDiamondsForUserQuery request, CancellationToken cancellationToken)
        {
            var accountId = AccountId.Parse(request.accountId);
            List<Diamond> diamonds = new();
            var getAllDiamonds = await _diamondRepository.GetUserLockDiamonds(accountId,cancellationToken);
            var getDiscounts = await _discountRepository.GetActiveDiscount();
            var getAllShape = await _diamondShapeRepository.GetAll();
            //var getRoundBrilliantPrice = await _diamondPriceRepository.GetPrice(false, true, cancellationToken);
            //var getRoundBrilliantPriceNatural = await _diamondPriceRepository.GetPrice(false, false, cancellationToken);

            //var getFancyPrice = await _diamondPriceRepository.GetPrice(true, true, cancellationToken);
            //var getFancyPriceNatural = await _diamondPriceRepository.GetPrice(true, false, cancellationToken);

            var getAllDiscount = await _discountRepository.GetActiveDiscount();
            foreach (var diamond in getAllDiamonds)
            {
                DiamondPrice diamondPrice;
                diamond.DiamondShape = getAllShape.FirstOrDefault(s => s.Id == diamond.DiamondShapeId);
                //if (DiamondShape.IsFancyShape(diamond.DiamondShapeId))
                //    diamondPrice = await _diamondServices.GetDiamondPrice(diamond, getFancyPrice);
                //else
                //    diamondPrice = await _diamondServices.GetDiamondPrice(diamond, getRoundBrilliantPrice);
                //_diamondServices.AssignDiamondDiscount(diamond, getAllDiscount).Wait();
                var diamondPriceByShape = await _diamondPriceRepository.GetPrice(diamond.Cut.Value,diamond.DiamondShape, diamond.IsLabDiamond, cancellationToken);
                diamondPrice = await _diamondServices.GetDiamondPrice(diamond, diamondPriceByShape);
            }
            return getAllDiamonds;
        }
    }
}

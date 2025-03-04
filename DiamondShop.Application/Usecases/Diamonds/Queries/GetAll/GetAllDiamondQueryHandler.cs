﻿using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetAll
{
    public record GetAllDiamondQuery() : IRequest<List<Diamond>>;
    internal class GetAllDiamondQueryHandler : IRequestHandler<GetAllDiamondQuery, List<Diamond>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly ILogger<GetAllDiamondQueryHandler> _logger;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IDiamondServices _diamondServices;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiscountRepository _discountRepository;

        public GetAllDiamondQueryHandler(IDiamondRepository diamondRepository, ILogger<GetAllDiamondQueryHandler> logger, IDiamondShapeRepository diamondShapeRepository, IDiamondServices diamondServices, IDiamondPriceRepository diamondPriceRepository, IDiscountRepository discountRepository)
        {
            _diamondRepository = diamondRepository;
            _logger = logger;
            _diamondShapeRepository = diamondShapeRepository;
            _diamondServices = diamondServices;
            _diamondPriceRepository = diamondPriceRepository;
            _discountRepository = discountRepository;
        }

        public async Task<List<Diamond>> Handle(GetAllDiamondQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("get all diamond");
            var result = (await _diamondRepository.GetAll()).Where(x => x.Status == Domain.Common.Enums.ProductStatus.Active).ToList();
            var getAllShape = await _diamondShapeRepository.GetAll();

            //var getRoundBrilliantPrice = await _diamondPriceRepository.GetPrice(false, true, cancellationToken);
            //var getFancyPrice = await _diamondPriceRepository.GetPrice(true, true, cancellationToken);
            //var getRoundBrilliantPriceNatural = await _diamondPriceRepository.GetPrice(false, false, cancellationToken);
            //var getFancyPriceNatural = await _diamondPriceRepository.GetPrice(true, false, cancellationToken);
            var getAllDiscount = await _discountRepository.GetActiveDiscount();
            foreach (var diamond in result)
            {
                DiamondPrice diamondPrice;
                diamond.DiamondShape = getAllShape.FirstOrDefault(s => s.Id == diamond.DiamondShapeId);
                if (diamond.IsLabDiamond)
                {
                    var prices = await _diamondPriceRepository.GetPrice(diamond.Cut,diamond.DiamondShape, true, cancellationToken);
                    diamondPrice = await _diamondServices.GetDiamondPrice(diamond, prices);
                    //if (DiamondShape.IsFancyShape(diamond.DiamondShapeId))
                    //    diamondPrice = await _diamondServices.GetDiamondPrice(diamond, getFancyPrice);
                    //else
                    //    diamondPrice = await _diamondServices.GetDiamondPrice(diamond, getRoundBrilliantPrice);
                }
                else
                {
                    var prices = await _diamondPriceRepository.GetPrice(diamond.Cut, diamond.DiamondShape, false, cancellationToken);
                    diamondPrice = await _diamondServices.GetDiamondPrice(diamond, prices);
                }
                _diamondServices.AssignDiamondDiscount(diamond, getAllDiscount).Wait();
            }
            return result;
        }
    }
}

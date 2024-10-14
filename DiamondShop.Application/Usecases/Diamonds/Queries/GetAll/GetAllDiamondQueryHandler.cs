using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Repositories;
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

        public GetAllDiamondQueryHandler(IDiamondRepository diamondRepository, ILogger<GetAllDiamondQueryHandler> logger, IDiamondShapeRepository diamondShapeRepository, IDiamondServices diamondServices, IDiamondPriceRepository diamondPriceRepository)
        {
            _diamondRepository = diamondRepository;
            _logger = logger;
            _diamondShapeRepository = diamondShapeRepository;
            _diamondServices = diamondServices;
            _diamondPriceRepository = diamondPriceRepository;
        }

        public async Task<List<Diamond>> Handle(GetAllDiamondQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("get all diamond");
            var result = (await _diamondRepository.GetAll()).ToList();
            var getAllShape = await _diamondShapeRepository.GetAll();
            Dictionary<string, List<DiamondPrice>> shapeDictPrice = new();
            foreach (var shape in getAllShape)
            {
                var prices = await _diamondPriceRepository.GetPriceByShapes(shape, cancellationToken);
                shapeDictPrice.Add(shape.Id.Value, prices);
            }
            foreach (var diamond in result)
            {
                diamond.DiamondShape = getAllShape.FirstOrDefault(s => s.Id == diamond.DiamondShapeId);
                var diamondPrice = await _diamondServices.GetDiamondPrice(diamond, shapeDictPrice.FirstOrDefault(d => d.Key == diamond.DiamondShapeId.Value).Value);
                diamond.DiamondPrice = diamondPrice;
            }
            //_diamondServices.CheckDiamondDiscount();
            return result;
        }
    }
}

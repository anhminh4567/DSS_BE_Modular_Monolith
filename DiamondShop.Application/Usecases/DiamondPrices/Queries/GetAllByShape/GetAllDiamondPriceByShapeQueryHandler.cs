using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Queries.GetAllByShape
{
    public record GetAllDiamondPriceByShapeQuery(Cut Cut,string shapeId,bool isLabDiamond = true) : IRequest<List<DiamondPrice>>;
    internal class GetAllDiamondPriceByShapeQueryHandler : IRequestHandler<GetAllDiamondPriceByShapeQuery, List<DiamondPrice>>
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;

        public GetAllDiamondPriceByShapeQueryHandler(IDiamondPriceRepository diamondPriceRepository, IDiamondShapeRepository diamondShapeRepository)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _diamondShapeRepository = diamondShapeRepository;
        }

        public async Task<List<DiamondPrice>> Handle(GetAllDiamondPriceByShapeQuery request, CancellationToken cancellationToken)
        {
            var shapeId = DiamondShapeId.Parse(request.shapeId);
            var getShape = await _diamondShapeRepository.GetById(shapeId);
            return await _diamondPriceRepository.GetPrice(request.Cut,getShape,request.isLabDiamond,cancellationToken);
        }
    }
}

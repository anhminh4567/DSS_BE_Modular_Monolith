using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Queries.GetSidePriceBoard
{
    public record GetSideDiamondPriceBoardQuery() : IRequest<Result<SideDiamondPriceBoardDto>>;
    internal class GetSideDiamondPriceBoardQueryHandler : IRequestHandler<GetSideDiamondPriceBoardQuery, Result<SideDiamondPriceBoardDto>>
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IMapper _mapper;

        public GetSideDiamondPriceBoardQueryHandler(IDiamondPriceRepository diamondPriceRepository, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondShapeRepository diamondShapeRepository, IMapper mapper)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _mapper = mapper;
        }
        public async Task<Result<SideDiamondPriceBoardDto>> Handle(GetSideDiamondPriceBoardQuery request, CancellationToken cancellationToken)
        {
            var getAllShape = await _diamondShapeRepository.GetAllIncludeSpecialShape(cancellationToken);
            var priceBoardMainShape = getAllShape.FirstOrDefault(s => s.Id == DiamondShape.ANY_SHAPES.Id);

            var getAllSideDiamondPrice = await _diamondPriceRepository.GetSideDiamondPrice(cancellationToken);
            var getAllGroup = await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCriteria(cancellationToken);
            var mappedGroup = getAllGroup.Select(x => ( CaratFrom :x.Key.CaratFrom, CaratTo : x.Key.CaratTo,  criteria : x.Value.FirstOrDefault() ) ).ToList();
            SideDiamondPriceBoardDto priceBoard = new SideDiamondPriceBoardDto();
            priceBoard.DiamondShape = _mapper.Map<DiamondShapeDto>(priceBoardMainShape);
            priceBoard.FullAllRowsWithUnknownPrice(mappedGroup.ToList());
            foreach(var price in getAllSideDiamondPrice)
            {
                priceBoard.MapPrice(price);
            }
            return priceBoard;
        }
    }
}

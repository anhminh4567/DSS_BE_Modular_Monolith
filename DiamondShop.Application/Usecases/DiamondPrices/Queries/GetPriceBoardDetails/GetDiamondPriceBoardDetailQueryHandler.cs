using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Usecases.DiamondPrices.Queries.GetPriceBoardBase;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Queries.GetPriceBoardDetails
{
    public record GetDiamondPriceBoardDetailQuery(GetDiamondPriceBoardBaseQuery basePrice) : IRequest<Result<DiamondPriceBoardDto>>;
    internal class GetDiamondPriceBoardDetailQueryHandler : IRequestHandler<GetDiamondPriceBoardDetailQuery, Result<DiamondPriceBoardDto>>
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly ISender _sender;
        private readonly IDiscountRepository _discountRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;

        public GetDiamondPriceBoardDetailQueryHandler(IDiamondPriceRepository diamondPriceRepository, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondShapeRepository diamondShapeRepository, ISender sender, IDiscountRepository discountRepository, IPromotionRepository promotionRepository, IMapper mapper)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _sender = sender;
            _discountRepository = discountRepository;
            _promotionRepository = promotionRepository;
            _mapper = mapper;
        }

        public async Task<Result<DiamondPriceBoardDto>> Handle(GetDiamondPriceBoardDetailQuery request, CancellationToken cancellationToken)
        {
            var activeDiscount = await _discountRepository.GetActiveDiscount();
            var getAllShape = await _diamondShapeRepository.GetAllIncludeSpecialShape();
            var getBasePriceBoard = await _sender.Send(request.basePrice);
            if(getBasePriceBoard.IsFailed)
                return Result.Fail(new NotFoundError("Base price not found"));
            var basePriceBoard = getBasePriceBoard.Value;
            var shape = getAllShape.FirstOrDefault(x => x.Id == DiamondShapeId.Parse( basePriceBoard.Shape.Id));
            basePriceBoard.PriceTables
               .AsParallel()
               .ForAll(x => x.MapDiscounts(activeDiscount, basePriceBoard.MainCut, shape, basePriceBoard.IsLabDiamondBoardPrices));
            return basePriceBoard;
            //
        }
    }
}

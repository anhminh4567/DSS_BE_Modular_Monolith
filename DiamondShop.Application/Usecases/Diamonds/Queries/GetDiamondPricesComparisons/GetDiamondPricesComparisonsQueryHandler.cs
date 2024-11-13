using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetDiamondPricesComparisons
{
    public record GetDiamondPricesComparisonsQuery(string shapeId,Diamond_4C Diamond_4C) : IRequest<Result<DiamondPricingFormatDto>>;
    internal class GetDiamondPricesComparisonsQueryHandler : IRequestHandler<GetDiamondPricesComparisonsQuery, Result<DiamondPricingFormatDto>>
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondServices _diamondServices;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public GetDiamondPricesComparisonsQueryHandler(IDiamondPriceRepository diamondPriceRepository, IDiamondRepository diamondRepository, IDiamondServices diamondServices, IDiamondShapeRepository diamondShapeRepository, IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _diamondRepository = diamondRepository;
            _diamondServices = diamondServices;
            _diamondShapeRepository = diamondShapeRepository;
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<DiamondPricingFormatDto>> Handle(GetDiamondPricesComparisonsQuery request, CancellationToken cancellationToken)
        {
            var parsedId = DiamondShapeId.Parse(request.shapeId);
            var getShape = (await _diamondShapeRepository.GetAll()).FirstOrDefault(x => x.Id == parsedId);
            if(getShape == null)
            {
                return Result.Fail("no shape found for this diamomd");
            }
            //, Diamond_Details Diamond_Details , Diamond_Measurement Diamond_Measurement
            var getPrices = await _diamondServices.GetPrice(request.Diamond_4C.Cut.Value,getShape,request.Diamond_4C.isLabDiamond,cancellationToken);
            var fakeDiamond = Diamond.Create(getShape, request.Diamond_4C,
                new Diamond_Details(Polish.Fair, Symmetry.Fair, Girdle.Thick, Fluorescence.Faint, Culet.Slightly_Large),
                new Diamond_Measurement(0.1f,0.1f,0.1f,"asdf"),
                1)  ;
            var getPrice = await _diamondServices.GetDiamondPrice(fakeDiamond, getPrices);
            fakeDiamond.DiamondPrice = null;
            fakeDiamond.DiamondShape = getShape;
            DiamondPricingFormatDto result = new();
            result.PriceFound = _mapper.Map<DiamondPriceDto>(getPrice);
            //result.Diamond = _mapper.Map<DiamondDto>(fakeDiamond);
            if(getPrice.ForUnknownPrice != null)//price is known
                return Result.Ok(result);
            
            var diamondRule = _optionsMonitor.CurrentValue.DiamondRule;

            result.CorrectPrice = fakeDiamond.TruePrice;
            //if(fakeDiamond.Cut!= null && fakeDiamond.Cut != Cut.Ideal)
            //{
            //    switch (fakeDiamond.Cut)
            //    {
            //        case Cut.Very_Good:
            //            result.CutOffsetFound += diamondRule.AverageOffsetVeryGoodCutFromIdealCut;
            //            break;
            //        case Cut.Good:
            //            result.CutOffsetFound += diamondRule.AverageOffsetGoodCutFromIdealCut;
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //result.SuggestedOffsetForDiamond = result.ShapeOffsetFound * result.CutOffsetFound;
            //result.SuggestedPrice = getPrice.Price * result.SuggestedOffsetForDiamond;
            return result;
        }
    }
}

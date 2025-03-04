﻿using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Domain.BusinessRules;
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
    public record GetDiamondPricesComparisonsQuery(string shapeId,decimal priceOffset,decimal? extraFee,Diamond_4C Diamond_4C) : IRequest<Result<DiamondPricingFormatDto>>;
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
            var getPrices = await _diamondServices.GetPrice(request.Diamond_4C.Cut,getShape,request.Diamond_4C.isLabDiamond,cancellationToken);
            var fakeDiamond = Diamond.Create(getShape, request.Diamond_4C,
                new Diamond_Details(Polish.Fair, Symmetry.Fair, Girdle.Thick, Fluorescence.Faint, Culet.Slightly_Large),
                new Diamond_Measurement(0.1f,0.1f,0.1f,"asdf"),request.priceOffset,null);
            var getPrice = await _diamondServices.GetDiamondPrice(fakeDiamond, getPrices);
            //fakeDiamond.DiamondPrice = null;
            fakeDiamond.DiamondShape = getShape;
            DiamondPricingFormatDto result = new();
            result.PriceFound = _mapper.Map<DiamondPriceDto>(getPrice);
            //result.Diamond = _mapper.Map<DiamondDto>(fakeDiamond);
            result.CorrectPrice = fakeDiamond.TruePrice;
            result.CurrentGivenOffset = request.priceOffset;
            result.Shape = _mapper.Map<DiamondShapeDto>(getShape);
            if (getPrice.ForUnknownPrice == null)//price is known{
            {
                result.IsPriceKnown = true;
                result.IsValid = true;
                result.Message = "Đã biết rõ giá, có thể so sánh với giá hiện tại";
                //return Result.Ok(result);
            }
            else 
            {
                result.IsPriceKnown = false;
                result.Message = "chưa rõ giá, bạn có muốn thêm vào ?";
                var extraFee =   (request.extraFee == null  ? 0 : request.extraFee );
                result.CorrectPrice += extraFee.Value;
            }
            var diamondRule = _optionsMonitor.CurrentValue.DiamondRule;
            bool isFancy = getShape.IsFancy();
            if (isFancy)
            {
                result.IsFancyShape = true;
                var offsetFound = diamondRule.GetFancyShapeOffset(getShape);
                if(offsetFound == null)
                {
                    result.FancyShapeOffsetSuggested = 0;
                }
                else
                {
                    result.FancyShapeOffsetSuggested = offsetFound.Value;
                }
            }
            else
            {
                result.IsFancyShape = false;
                if (request.Diamond_4C.Cut != null)
                {
                    var cutOffsetFound = diamondRule.GetCutOffset(request.Diamond_4C.Cut.Value);
                    if (cutOffsetFound != null)
                        result.CutOffsetSuggested = cutOffsetFound.Value;
                    else
                        result.CutOffsetSuggested = 0;
                }
                else
                    result.CutOffsetSuggested = 0;
            }
            result.SuggestedOffsetTobeAdded = Math.Round((result.CutOffsetSuggested + result.FancyShapeOffsetSuggested) / 2m, 2);;
            return result;
        }
    }
}


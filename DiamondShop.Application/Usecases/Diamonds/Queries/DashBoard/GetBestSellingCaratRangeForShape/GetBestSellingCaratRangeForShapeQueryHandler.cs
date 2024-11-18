using Azure;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Dtos.Responses.Diamonds.DashboardDto;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.DashBoard.GetBestSellingCaratRangeForShape
{
    public record GetBestSellingCaratRangeForShapeQuery(string shapeId, float caratFrom, float caratTo, string? startDate, string? endDate) : IRequest<Result<DiamondBestSellingCaratRangePerShapeDto>>;
    internal class GetBestSellingCaratRangeForShapeQueryHandler : IRequestHandler<GetBestSellingCaratRangeForShapeQuery, Result<DiamondBestSellingCaratRangePerShapeDto>>
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IDiamondServices _diamondServices;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IMapper _mapper;

        public GetBestSellingCaratRangeForShapeQueryHandler(IDiamondPriceRepository diamondPriceRepository, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondShapeRepository diamondShapeRepository, IDiamondServices diamondServices, IDiamondRepository diamondRepository, IMapper mapper)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _diamondServices = diamondServices;
            _diamondRepository = diamondRepository;
            _mapper = mapper;
        }

        public async Task<Result<DiamondBestSellingCaratRangePerShapeDto>> Handle(GetBestSellingCaratRangeForShapeQuery request, CancellationToken cancellationToken)
        {
            var response = new DiamondBestSellingCaratRangePerShapeDto();
            DiamondShapeId shapeId = DiamondShapeId.Parse(request.shapeId);
            var getAllShape = await _diamondShapeRepository.GetAll();
            var getShape = getAllShape.FirstOrDefault(x => x.Id == shapeId);
            if (getShape is null)
                return Result.Fail(new NotFoundError("Shape not found"));
            bool isFancyShape = getShape.IsFancy();
            var cutGrades = CutHelper.GetCutList();
            var parsedStartResults = DateTime.TryParseExact(request.startDate, DateTimeFormatingRules.DateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime startParsed);
            var parsedEndResults = DateTime.TryParseExact(request.endDate, DateTimeFormatingRules.DateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime endParsed);
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (parsedStartResults)
            {
                startDate = startParsed.ToUniversalTime();
                response.From = _mapper.Map<string>(startDate);
            }
            if (parsedEndResults)
            {
                endDate = endParsed.ToUniversalTime();
                response.To = _mapper.Map<string>(endDate);
            }
            response.Shape = _mapper.Map<DiamondShapeDto>(getShape);
            var getSoldShapes = await _diamondRepository.GetTotalSoldDiamondsByShape(getShape);
            var totalSold = getSoldShapes.Count;
            var totalRevenue = getSoldShapes.Sum(x => x.SoldPrice);
            List<string> soldIds = _mapper.Map<List<string>>(getSoldShapes.Select(x => x.Id));
            response.TotalRevenue = totalRevenue.Value;
            response.TotalSold = totalSold;
            response.CaratFrom = request.caratFrom;
            response.CaratTo = request.caratTo;
            return response;
        }
    }
}

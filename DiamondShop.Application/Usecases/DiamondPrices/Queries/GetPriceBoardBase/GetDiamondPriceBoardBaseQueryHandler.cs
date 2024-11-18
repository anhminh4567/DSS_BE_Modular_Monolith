using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MapsterMapper;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Queries.GetPriceBoardBase
{
    // cut is not required, might leave it as it be
    //
    public record GetDiamondPriceBoardBaseQuery(Cut cut, bool isLabDiamond, bool isSideDiamond, string? shapeId) : IRequest<Result<DiamondPriceBoardDto>>;// bool IsSideDiamond = false
    internal class GetDiamondPriceBoardBaseQueryHandler : IRequestHandler<GetDiamondPriceBoardBaseQuery, Result<DiamondPriceBoardDto>>
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;

        public GetDiamondPriceBoardBaseQueryHandler(IDiamondPriceRepository diamondPriceRepository, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondShapeRepository diamondShapeRepository, IDiscountRepository discountRepository, IMapper mapper)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _discountRepository = discountRepository;
            _mapper = mapper;
        }

        public async Task<Result<DiamondPriceBoardDto>> Handle(GetDiamondPriceBoardBaseQuery request, CancellationToken cancellationToken)
        {
            var activeDiscount = await _discountRepository.GetActiveDiscount();
            if (request.isSideDiamond == false && request.shapeId is null)
                return Result.Fail(new ValidationError("shapeId is required for main diamond"));
            var shapeId = DiamondShapeId.Parse(request.shapeId);
            var getAllShape = await _diamondShapeRepository.GetAllIncludeSpecialShape();
            DiamondShape getShape = getAllShape.FirstOrDefault(x => x.Id == shapeId);
            if (request.isSideDiamond)
                getShape = getAllShape.FirstOrDefault(s => s.Id == DiamondShape.ANY_SHAPES.Id);
            if (getShape is null)
                return Result.Fail(new NotFoundError("Shape not found"));

            //DiamondShape priceBoardMainShape;
            //if (request.isFancyShapePrice)
            //{
            //    priceBoardMainShape = await _diamondShapeRepository.GetById(DiamondShape.FANCY_SHAPES.Id); //DiamondShape.FANCY_SHAPES;
            //}
            //else
            //{
            //    priceBoardMainShape = await _diamondShapeRepository.GetById(DiamondShape.ROUND.Id);//DiamondShape.ROUND;
            //}
            List<DiamondPrice> prices = new();
            Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>> criteriasByGrouping = new();
            DiamondPriceBoardDto priceBoard = DiamondPriceBoardDto.Create();
            priceBoard.MainCut = Cut.Excellent;
            priceBoard.Shape = _mapper.Map<DiamondShapeDto>(getShape);
            priceBoard.IsLabDiamondBoardPrices = request.isLabDiamond;
            if (request.isSideDiamond == false)
            {
                bool isFancyShape = DiamondShape.IsFancyShape(getShape.Id);

                prices = await _diamondPriceRepository.GetPrice(request.cut, getShape, request.isLabDiamond, cancellationToken);
                priceBoard.IsSideDiamondBoardPrices = false;
                string serlized = JsonConvert.SerializeObject(prices, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                byte[] bytes = Encoding.UTF8.GetBytes(serlized);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("size: " + bytes.Length);
                Console.ResetColor();
                //criteriasCarat = await _diamondCriteriaRepository.GroupAllAvailableCaratRange( cancellationToken);
                priceBoard.MainCut = request.cut;
                if (isFancyShape)
                    criteriasByGrouping = await _diamondCriteriaRepository.GroupAllAvailableCriteria(getShape, null, cancellationToken);
                else
                    criteriasByGrouping = await _diamondCriteriaRepository.GroupAllAvailableCriteria(getShape, priceBoard.MainCut, cancellationToken);
            }
            else
            {
                var isFancy = DiamondShape.IsFancyShape(getShape.Id);
                prices = await _diamondPriceRepository.GetSideDiamondPrice(request.isLabDiamond, cancellationToken);
                priceBoard.IsSideDiamondBoardPrices = true;
                priceBoard.Shape = _mapper.Map<DiamondShapeDto>(DiamondShape.ANY_SHAPES);
                //criteriasCarat = await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCaratRange(cancellationToken);
                criteriasByGrouping = await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCriteria(cancellationToken);
            }
            //criteriasCarat = criteriasCarat.OrderBy(x => x.CaratTo).ToList();



            Stopwatch stopwatch = Stopwatch.StartNew();
            var diamondCaratRangeGrouped = prices
                .GroupBy(dp => new { dp.Criteria.CaratFrom, dp.Criteria.CaratTo })
                .OrderBy(dp => dp.Key.CaratTo)
                .ToList();
            var groupByCaratRangeFromPrices = diamondCaratRangeGrouped
                .Select(dcr => new
                {
                    TableRange = dcr.Key,
                    TableItem = prices.Where(x =>
                        x.Criteria.CaratFrom == dcr.Key.CaratFrom &&
                        x.Criteria.CaratTo == dcr.Key.CaratTo).ToList()
                })
            .ToList();
            Dictionary<Color, int> colorRange = criteriasByGrouping.SelectMany(x => x.Value)
                .Select(x => x.Color)
                .Distinct()
                .OrderBy(x => x.Value).ToDictionary(x => x.Value, x => (int)x.Value);
            Dictionary<Clarity, int> clarityRange = criteriasByGrouping.SelectMany(x => x.Value)
                .Select(x => x.Clarity)
                .Distinct()
                .OrderBy(x => x.Value).ToDictionary(x => x.Value, x => (int)x.Value);

            var createTable = criteriasByGrouping
                .AsParallel()
                .AsOrdered()
                .Select(dp => new DiamondPriceTableDto()
                {
                    CaratFrom = dp.Key.CaratFrom,
                    CaratTo = dp.Key.CaratTo,
                    ColorRange = colorRange,
                    ClarityRange = clarityRange,
                    GroupedCriteria = dp.Value
                }).ToList();
            createTable.ForEach(x =>
            {
                x.CellMatrix = new DiamondPriceCellDataDto[colorRange.Count, clarityRange.Count];
                x.FillAllCellsWithUnknonwPrice();

            });

            createTable.ForEach(x =>
            {
                var getCorrectGroupPrice = groupByCaratRangeFromPrices.FirstOrDefault(gp => gp.TableRange.CaratFrom == x.CaratFrom && gp.TableRange.CaratTo == x.CaratTo);
                if (getCorrectGroupPrice == null) { }
                else
                {
                    var prices = getCorrectGroupPrice.TableItem;
                    x.MapPriceToCells(prices);
                }
                if (x.IsAnyPriceUncover)
                    priceBoard.UncoveredTableCaratRange.Add((x.CaratFrom, x.CaratTo));
            });

            priceBoard.PriceTables = createTable;
            //assign discount on table
            //priceBoard.PriceTables
            //    .AsParallel()
            //    .ForAll(x => x.MapDiscounts(activeDiscount, priceBoard.MainCut, getShape, priceBoard.IsLabDiamondBoardPrices));
            ////
            stopwatch.Stop();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("execution time measured in miliseconds: " + stopwatch.ElapsedMilliseconds);
            Console.ResetColor();
            return priceBoard;
        }
    }
}

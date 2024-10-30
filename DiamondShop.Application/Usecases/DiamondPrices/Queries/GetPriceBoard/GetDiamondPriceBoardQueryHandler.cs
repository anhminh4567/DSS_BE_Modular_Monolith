using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MapsterMapper;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Queries.GetPriceBoard
{
    // cut is not required, might leave it as it be
    public record GetDiamondPriceBoardQuery(string shapeId, bool isLabDiamond, Cut? Cut = Cut.Excelent, bool IsSideDiamond = false) : IRequest<Result<DiamondPriceBoardDto>>;
    internal class GetDiamondPriceBoardQueryHandler : IRequestHandler<GetDiamondPriceBoardQuery, Result<DiamondPriceBoardDto>>
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IMapper _mapper;

        public GetDiamondPriceBoardQueryHandler(IDiamondPriceRepository diamondPriceRepository, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondShapeRepository diamondShapeRepository, IMapper mapper)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _mapper = mapper;
        }

        public async Task<Result<DiamondPriceBoardDto>> Handle(GetDiamondPriceBoardQuery request, CancellationToken cancellationToken)
        {
            var shapeId = DiamondShapeId.Parse(request.shapeId);
            var getShape = await _diamondShapeRepository.GetById(shapeId);
            if (getShape is null)
                return Result.Fail(new NotFoundError("Shape not found"));
            List<DiamondPrice> prices = new();
            //List<(float CaratFrom, float CaratTo)> criteriasCarat = new();
            Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>> criteriasByGrouping = new();

            DiamondPriceBoardDto priceBoard = DiamondPriceBoardDto.Create();
            priceBoard.MainCut = request.Cut.Value;
            priceBoard.Shape = _mapper.Map<DiamondShapeDto>(getShape);
            priceBoard.IsLabDiamondBoardPrices = request.isLabDiamond;
            if (request.IsSideDiamond == false)
            {
                prices = await _diamondPriceRepository.GetPriceByShapes(getShape, request.isLabDiamond, cancellationToken);
                priceBoard.IsSideDiamondBoardPrices = false;
                //criteriasCarat = await _diamondCriteriaRepository.GroupAllAvailableCaratRange( cancellationToken);
                criteriasByGrouping = (await _diamondCriteriaRepository.GroupAllAvailableCriteria(cancellationToken));

            }
            else
            {
                prices = await _diamondPriceRepository.GetSideDiamondPrice(request.isLabDiamond, cancellationToken);
                priceBoard.Shape = _mapper.Map<DiamondShapeDto>(DiamondShape.AnyShape);
                priceBoard.IsSideDiamondBoardPrices = true;
                //criteriasCarat = await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCaratRange(cancellationToken);
                criteriasByGrouping = (await _diamondCriteriaRepository.GroupAllAvailableSideDiamondCriteria(cancellationToken));
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
            Dictionary<Color, int> colorRange = groupByCaratRangeFromPrices
                .SelectMany(x => x.TableItem)
                .Select(x => x.Criteria.Color)
                .Distinct()
                .OrderBy(x => x.Value).ToDictionary(x => x.Value, x => ((int)x.Value));
            Dictionary<Clarity, int> clarityRange = groupByCaratRangeFromPrices
                .SelectMany(x => x.TableItem)
                .Select(x => x.Criteria.Clarity)
                .Distinct()
                .OrderBy(x => x.Value).ToDictionary(x => x.Value, x => ((int) x.Value));


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
            } );

            createTable.ForEach(x =>
            {
                var getCorrectGroupPrice = groupByCaratRangeFromPrices.FirstOrDefault(gp => gp.TableRange.CaratFrom == x.CaratFrom&& gp.TableRange.CaratTo == x.CaratTo);
                if (getCorrectGroupPrice == null) { }
                else 
                {
                    var prices = getCorrectGroupPrice.TableItem;
                    x.MapPriceToCells(prices);
                }
            });
            
            priceBoard.PriceTables = createTable;
            stopwatch.Stop();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("execution time measured in miliseconds: " + stopwatch.ElapsedMilliseconds);
            Console.ResetColor();
            return priceBoard;
            //var createTable = groupByCaratRangeFromPrices
            //    .AsParallel()
            //    //.AsOrdered()
            //    .Select(dp => new DiamondPriceTableDto()
            //    {
            //        CaratFrom = dp.TableRange.CaratFrom,
            //        CaratTo = dp.TableRange.CaratTo,
            //        ColorRange = colorRange,
            //        ClarityRange = clarityRange,
            //        PriceCells = dp.TableItem
            //            .Select(x => new DiamondPriceCellDataDto()
            //            {
            //                CriteriaId = x.Criteria.Id.Value,
            //                Color = x.Criteria.Color.Value,
            //                Clarity = x.Criteria.Clarity.Value,
            //                Price = x.Price
            //            }).ToList()
            //    }).ToList();
            //priceBoard.PriceTables = createTable;
            //var anyMissingCaratRangeNotCovered = createTable
            //    .GroupBy(x => new { x.CaratFrom, x.CaratTo })
            //    .Select(x => (x.Key.CaratFrom, x.Key.CaratTo))
            //    .ToList();
            //var caratNotInCriteria = criteriasCarat.Except(anyMissingCaratRangeNotCovered).ToList();
            //priceBoard.UncoveredTableCaratRange.AddRange(caratNotInCriteria);
            //priceBoard.UncoveredTableCaratRange.ForEach(x =>
            //{
            //    priceBoard.PriceTables.Add(new DiamondPriceTableDto()
            //    {
            //        CaratFrom = x.CaratFrom,
            //        CaratTo = x.CaratTo,
            //        ClarityRange = clarityRange,
            //        ColorRange = colorRange,
            //        PriceCells = new List<DiamondPriceCellDataDto>()
            //    }) ;
            //});
            //var tables = priceBoard.PriceTables;
            //Parallel.ForEach(priceBoard.PriceTables, table => table.FillMissingCells());


        }
    }
}

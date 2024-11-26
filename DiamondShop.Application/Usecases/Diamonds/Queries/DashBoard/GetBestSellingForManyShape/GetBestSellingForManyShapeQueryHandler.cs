using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Dtos.Responses.Diamonds.DashboardDto;
using DiamondShop.Application.Usecases.Diamonds.Queries.DashBoard.GetBestSellingCaratRangeForShape;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.DashBoard.GetBestSellingForManyShape
{
    public record GetBestSellingForShapeQuery(string? startDate, string? endDate, bool? islab) : IRequest<Result<ListBestSellingDiamondShapeDto>>;
    internal class GetBestSellingForManyShapeQueryHandler : IRequestHandler<GetBestSellingForShapeQuery, Result<ListBestSellingDiamondShapeDto>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IMapper _mapper;

        public GetBestSellingForManyShapeQueryHandler(IDiamondRepository diamondRepository, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondPriceRepository diamondPriceRepository, IDiamondShapeRepository diamondShapeRepository, IMapper mapper)
        {
            _diamondRepository = diamondRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _mapper = mapper;
        }

        public async Task<Result<ListBestSellingDiamondShapeDto>> Handle(GetBestSellingForShapeQuery request, CancellationToken cancellationToken)
        {
            var response = new ListBestSellingDiamondShapeDto();
            var getAllShape = await _diamondShapeRepository.GetAll();
            var getAllDiamonds = await _diamondRepository.GetAll();
            if (request.islab != null)
            {
                getAllDiamonds = getAllDiamonds.Where(x => x.IsLabDiamond == request.islab).ToList();
            }
            var getAllDiamondCounts = getAllDiamonds.Count(); //_diamondRepository.GetCount();
            var parsedStartResults = DateTime.TryParseExact(request.startDate, DateTimeFormatingRules.DateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime startParsed);
            var parsedEndResults = DateTime.TryParseExact(request.endDate, DateTimeFormatingRules.DateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime endParsed);

            response.TotalInStock = getAllDiamondCounts;
            response.TotalActive = getAllDiamonds.Where(x => x.Status == ProductStatus.Active).Count(); //await _diamondRepository.GetCountByStatus(new List<ProductStatus> { ProductStatus.Active },request.islab);
            response.TotalInactive = getAllDiamonds.Where(x => x.Status == ProductStatus.Inactive).Count(); //await _diamondRepository.GetCountByStatus(new List<ProductStatus> { ProductStatus.Inactive }, request.islab);
            foreach (var shape in getAllShape)
            {
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
                var soldDiamonds = await _diamondRepository.GetTotalSoldDiamondsByShape(shape,request.islab, startDate, endDate);
                response.DiamondBestSellingShapes.Add(new DiamondBestSellingShapeDto
                {
                    Shape = _mapper.Map<DiamondShapeDto>(shape),
                    TotalActive = _diamondRepository.GetCountByShapeAndStatus(new List<ProductStatus> { ProductStatus.Active }, request.islab, new List<DiamondShapeId> { shape.Id }).Result,
                    TotalInactive = _diamondRepository.GetCountByShapeAndStatus(new List<ProductStatus> { ProductStatus.Inactive }, request.islab, new List<DiamondShapeId> { shape.Id }).Result,
                    TotalInStock = _diamondRepository.GetCountByShapeAndStatus(ProductStatusHelper.GetAllStatus(), request.islab, new List<DiamondShapeId> { shape.Id }).Result,
                    TotalLock = _diamondRepository.GetCountByShapeAndStatus(new List<ProductStatus> { ProductStatus.Locked }, request.islab, new List<DiamondShapeId> { shape.Id }).Result,
                    TotalLockForUser = _diamondRepository.GetCountByShapeAndStatus(new List<ProductStatus> { ProductStatus.LockForUser }, request.islab, new List<DiamondShapeId> { shape.Id }).Result,
                    TotalRevenueForThisShape = soldDiamonds.Sum(x => x.SoldPrice).Value,
                    TotalSold = _diamondRepository.GetCountByShapeAndStatus(new List<ProductStatus> { ProductStatus.Sold }, request.islab, new List<DiamondShapeId> { shape.Id }).Result,
                });
            }
            response.TotalLocked = response.DiamondBestSellingShapes.Sum(x => x.TotalLock);
            response.TotalSold = response.DiamondBestSellingShapes.Sum(x => x.TotalSold);
            return response;
            throw new NotImplementedException();
        }
    }

}

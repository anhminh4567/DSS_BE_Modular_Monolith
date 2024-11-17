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
    public record GetBestSellingForShapeQuery(string startDate , string endDate) : IRequest<Result<ListBestSellingDiamondShapeDto>>;
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
            var getAllDiamondCounts = _diamondRepository.GetCount();
            var parsedStartResults = DateTime.TryParseExact(request.startDate, DateTimeFormatingRules.DateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime startParsed);
            var parsedEndResults = DateTime.TryParseExact(request.endDate, DateTimeFormatingRules.DateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime endParsed);
            response.From = startParsed;
            response.To = endParsed;
            response.TotalInStock = getAllDiamondCounts;
            response.TotalActive = await _diamondRepository.GetCountByStatus(new List<ProductStatus> { ProductStatus.Active });
            response.TotalInactive = await _diamondRepository.GetCountByStatus(new List<ProductStatus> { ProductStatus.Inactive });
            foreach (var shape in getAllShape)
            {
                var soldDiamonds = await _diamondRepository.GetTotalSoldDiamondsByShape(shape, startParsed, endParsed);
                response.DiamondBestSellingShapes.Add(new DiamondBestSellingShapeDto
                {
                    Shape = _mapper.Map<DiamondShapeDto>(shape),
                    TotalActive =  _diamondRepository.GetCountByShapeAndStatus(new List<ProductStatus> { ProductStatus.Active }, new List<DiamondShapeId> { shape.Id }).Result,
                    TotalInactive =  _diamondRepository.GetCountByShapeAndStatus(new List<ProductStatus> { ProductStatus.Inactive }, new List<DiamondShapeId> { shape.Id }).Result,
                    TotalInStock = _diamondRepository.GetCountByShapeAndStatus(ProductStatusHelper.GetAllStatus(), new List<DiamondShapeId> { shape.Id }).Result,
                    TotalLock =  _diamondRepository.GetCountByShapeAndStatus(new List<ProductStatus> { ProductStatus.Locked }, new List<DiamondShapeId> { shape.Id }).Result,
                    TotalLockForUser = _diamondRepository.GetCountByShapeAndStatus(new List<ProductStatus> { ProductStatus.LockForUser }, new List<DiamondShapeId> { shape.Id }).Result,
                    TotalRevenueForThisShape = soldDiamonds.Sum(x => x.SoldPrice).Value,
                    TotalSold = _diamondRepository.GetCountByShapeAndStatus(new List<ProductStatus> { ProductStatus.Sold},new List<DiamondShapeId> { shape.Id}).Result, 
                });
            }
            return response;
            throw new NotImplementedException();
        }
    }

}

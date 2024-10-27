using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore; 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Queries.GetPaging
{
    public record GetDiamondPricePagingQuery(string diamondShapeId,int pageSize = 100, int start = 0 ) : IRequest<PagingResponseDto<DiamondPrice>>;
    internal class GetDiamondPricePagingQueryHandler : IRequestHandler<GetDiamondPricePagingQuery, PagingResponseDto<DiamondPrice>>
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly ILogger<GetDiamondPricePagingQueryHandler> _logger;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;

        public GetDiamondPricePagingQueryHandler(IDiamondPriceRepository diamondPriceRepository, ILogger<GetDiamondPricePagingQueryHandler> logger, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondShapeRepository diamondShapeRepository)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _logger = logger;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondShapeRepository = diamondShapeRepository;
        }

        public async Task<PagingResponseDto<DiamondPrice>> Handle(GetDiamondPricePagingQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("call get paging price");
            var diamondShapeId = DiamondShapeId.Parse(request.diamondShapeId);
            var tryGetShape = await _diamondShapeRepository.GetById(diamondShapeId);
            if (tryGetShape == null)
                throw new Exception("shape not exist");
            var query = _diamondPriceRepository.GetQuery();
            query = _diamondPriceRepository.QueryFilter(query , price => price.ShapeId == tryGetShape.Id);
            query = _diamondPriceRepository.QueryFilter(query, price => price.CriteriaId != null);

            query = _diamondPriceRepository.QueryInclude(query, include1 => include1.Shape);
            query = _diamondPriceRepository.QueryInclude(query, include2 => include2.Criteria);
            query = _diamondPriceRepository.QuerySplit(query);
            query = query.Skip(request.pageSize * request.start);
            query = query.Take(request.pageSize);
            var result = query.ToList();
            //var totalCount = _diamondPriceRepository
            //    .QueryFilter (_diamondPriceRepository.GetQuery(), price => price.ShapeId == tryGetShape.Id)
            //    .Count();
            var currentPage = request.start + 1;
            return new PagingResponseDto<DiamondPrice>(
                 TotalPage: 99999,
                 CurrentPage: currentPage,
                 Values: result);
           
        }
    }
}

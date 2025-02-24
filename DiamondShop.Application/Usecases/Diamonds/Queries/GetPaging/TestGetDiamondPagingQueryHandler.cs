using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetPaging
{
    public record TestGetDiamondPagingQuery(bool? isLab, string? shapeId, bool? includeJewelryDiamond, int pageSize = 20, int start = 0, decimal priceStart = 0, decimal priceEnd = decimal.MaxValue,
     GetDiamond_4C? diamond_4C = null, GetDiamond_Details? diamond_Details = null, GetDiamond_ManagerQuery? GetDiamond_ManagerQuery = null) : IRequest<Result<PagingResponseDto<Diamond>>>;
    internal class TestGetDiamondPagingQueryHandler : IRequestHandler<TestGetDiamondPagingQuery, Result<PagingResponseDto<Diamond>>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondServices _diamondService;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IDiscountService _discountService;
        private readonly IDiscountRepository _discountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepository _accountRepository;
        private readonly IDiamondRequestRepository _diamondRequestRepository;
        public TestGetDiamondPagingQueryHandler(IDiamondRepository diamondRepository, IDiamondPriceRepository diamondPriceRepository, IDiamondServices diamondService, IDiamondShapeRepository diamondShapeRepository, IDiscountService discountService, IDiscountRepository discountRepository, IHttpContextAccessor httpContextAccessor, IAccountRepository accountRepository, IDiamondRequestRepository diamondRequestRepository)
        {
            _diamondRepository = diamondRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondService = diamondService;
            _diamondShapeRepository = diamondShapeRepository;
            _discountService = discountService;
            _discountRepository = discountRepository;
            _httpContextAccessor = httpContextAccessor;
            _accountRepository = accountRepository;
            _diamondRequestRepository = diamondRequestRepository;
        }

        public async Task<Result<PagingResponseDto<Diamond>>> Handle(TestGetDiamondPagingQuery request, CancellationToken cancellationToken)
        {
            var parsedShapeId = string.IsNullOrEmpty(request.shapeId) ? null : DiamondShapeId.Parse(request.shapeId);
            GetDiamond_4C? diamond_4C = request.diamond_4C;
            if (request.diamond_4C == null)
            {
                diamond_4C = new GetDiamond_4C();
            }
            var trueStart = request.start * request.pageSize;
            var getAllShape = await _diamondShapeRepository.GetAll();
            var getActiveDiscount = await _discountRepository.GetActiveDiscount();
            var query = _diamondRepository.GetQuery();
            if (parsedShapeId is not null)
                query = query.Where(d => d.DiamondShapeId == parsedShapeId);

            if (AccountRole.ShopRoles.Any(x => _httpContextAccessor.HttpContext.User.IsInRole(x.Id.Value)) is false)//not in shop
            {
                query = _diamondRepository.QueryFilter(query, d => d.Status == Domain.Common.Enums.ProductStatus.Active);
            }
            else
            {
                var managerQuery = request.GetDiamond_ManagerQuery;
                if (managerQuery != null)
                {
                    if (managerQuery.diamondStatuses is not null)
                        query = _diamondRepository.QueryStatus(query, managerQuery.diamondStatuses);
                }
            }
            if (request.includeJewelryDiamond is not null)
            {
                if (request.includeJewelryDiamond == false)
                    query = _diamondRepository.QueryFilter(query, d => d.JewelryId == null);
                else
                    query = _diamondRepository.QueryFilter(query, d => d.JewelryId != null);
            }
            if (request.isLab != null)
                query = _diamondRepository.QueryFilter(query, d => d.IsLabDiamond == request.isLab);
            if (parsedShapeId is not null)
                query = query.Where(d => d.DiamondShapeId == parsedShapeId);
            if (request.diamond_4C is not null)
                query = _diamondRepository.Filtering4C(query, request.diamond_4C);
            if (request.diamond_Details is not null)
                query = _diamondRepository.FilteringDetail(query, request.diamond_Details);
            //query = _diamondRepository.QueryOrderBy( query,x => query.OrderBy(x => x.Carat));
            if (request.GetDiamond_ManagerQuery is not null)
            {
                if (request.GetDiamond_ManagerQuery.sku is not null)
                {
                    var getResult = await _diamondRepository.GetWhereSkuContain(_diamondRepository.GetQuery(), request.GetDiamond_ManagerQuery.sku);
                    //var getResult = await _diamondRepository.GetWhereSkuContain(request.GetDiamond_ManagerQuery.sku, trueStart,)
                    var skuTotalCount = getResult.Count();
                    var skuList = getResult.Skip(trueStart).Take(request.pageSize).ToList();
                    foreach (var diamond in skuList)
                    {
                        DiamondPrice diamondPrice;
                        diamond.DiamondShape = getAllShape.First(s => s.Id == diamond.DiamondShapeId);
                        var diamondPriceBySHape = await _diamondPriceRepository.GetPrice(diamond.Cut, diamond.DiamondShape, diamond.IsLabDiamond);
                        diamondPrice = await _diamondService.GetDiamondPrice(diamond, diamondPriceBySHape);
                        var diamondRequest = await _diamondRequestRepository.GetByDiamondId(diamond.Id);
                        _diamondService.AssignDiamondDiscount(diamond, getActiveDiscount).Wait();
                        if (diamondRequest is not null)
                            diamond.DiamondRequest = diamondRequest;
                    }
                    var skutotalPage = (int)Math.Ceiling((decimal)skuTotalCount / (decimal)request.pageSize);
                    return new PagingResponseDto<Diamond>(
                    TotalPage: skutotalPage,
                    CurrentPage: request.start,
                    Values: skuList,
                    TotalCount: skuTotalCount,
                    TotalTake: skuList.Count);
                }
            }
            var resultQuery = await _diamondRepository.FilteringPrice(query, diamond_4C, request.priceStart, request.priceEnd);
            var totalCount = resultQuery.Count();
            var returnList = resultQuery.Skip(trueStart).Take(request.pageSize).ToList();
            var getCount = returnList.Count();
            foreach (var diamond in returnList)
            {
                DiamondPrice diamondPrice;
                diamond.DiamondShape = getAllShape.First(s => s.Id == diamond.DiamondShapeId);
                var diamondPriceBySHape = await _diamondPriceRepository.GetPrice(diamond.Cut, diamond.DiamondShape, diamond.IsLabDiamond);
                diamondPrice = await _diamondService.GetDiamondPrice(diamond, diamondPriceBySHape);
                _diamondService.AssignDiamondDiscount(diamond, getActiveDiscount).Wait();
                var diamondRequest = await _diamondRequestRepository.GetByDiamondId(diamond.Id);
                if (diamondRequest is not null)
                    diamond.DiamondRequest = diamondRequest;
            }
            //var pageTake = finalQuery.Skip(request.start * request.pageSize).Take(request.pageSize).ToList();
            var selectLockedDiamond = returnList
                .Where(x => x.Status == Domain.Common.Enums.ProductStatus.LockForUser && x.ProductLock != null)
                .ToList();
            var getAccounts = await _accountRepository.GetAccounts(selectLockedDiamond.Select(x => x.ProductLock.AccountId).ToList());
            var totalPage = (int)Math.Ceiling((decimal)totalCount / (decimal)request.pageSize);
            var response = new PagingResponseDto<Diamond>(
                TotalPage: totalPage,
                CurrentPage: request.start,
                Values: returnList,
                TotalCount: totalCount,
                TotalTake: getCount);
            return response;
            throw new NotImplementedException();
        }

    }
}

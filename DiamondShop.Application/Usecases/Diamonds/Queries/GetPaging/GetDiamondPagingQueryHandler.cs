using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Usecases.Carts.Commands.Add;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetPaging
{

    internal class GetDiamondPagingQueryHandler : IRequestHandler<GetDiamondPagingQuery, Result<PagingResponseDto<Diamond>>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondServices _diamondService;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IDiscountService _discountService;
        private readonly IDiscountRepository _discountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepository _accountRepository;
        private List<Discount> ActiveDiscount = new();
        private List<DiamondShape> Shapes = new();
        private int Count = 0;
        private bool IncludeJewelryDiamond = false;
        private int TotalTake = 0;
        private int MaxInDb = 0;

        public GetDiamondPagingQueryHandler(IDiamondRepository diamondRepository, IDiamondPriceRepository diamondPriceRepository, IDiamondServices diamondService, IDiamondShapeRepository diamondShapeRepository, IDiscountService discountService, IDiscountRepository discountRepository, IHttpContextAccessor httpContextAccessor, IAccountRepository accountRepository)
        {
            _diamondRepository = diamondRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondService = diamondService;
            _diamondShapeRepository = diamondShapeRepository;
            _discountService = discountService;
            _discountRepository = discountRepository;
            _httpContextAccessor = httpContextAccessor;
            _accountRepository = accountRepository;
        }

        public async Task<Result<PagingResponseDto<Diamond>>> Handle(GetDiamondPagingQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out bool? isLab, out string? shapeId, out bool? includeJewelryDiamond, out int pageSize, out int start, out decimal priceStart, out var priceEnd, out var diamond_4C, out var diamond_Details, out var getDiamond_ManagerQuery);
            var query = _diamondRepository.GetQuery();

            List<Diamond> diamondListResponse = new();
            IncludeJewelryDiamond = includeJewelryDiamond.Value;

            var getAllShape = await _diamondShapeRepository.GetAll();
            Shapes = getAllShape;
            var getActiveDiscount = await _discountRepository.GetActiveDiscount();
            ActiveDiscount = getActiveDiscount;
            if (AccountRole.ShopRoles.Any(x => _httpContextAccessor.HttpContext.User.IsInRole(x.Id.Value)) is false)//not in shop
            {
                if (isLab == null || shapeId == null)
                    return Result.Fail("người dùng bắt buộc phải điền lab và shape id, chỉ có nhân viên shop mới được để trống");
                query = _diamondRepository.QueryFilter(query, d => d.Status == Domain.Common.Enums.ProductStatus.Active);
            }
            //var count = _diamondRepository.GetCount();
            //Count = count;
            TotalTake = (start + 1) * pageSize;
            MaxInDb = _diamondRepository.GetCount();
            var finalResult = await GetData(diamondListResponse, 0, pageSize, 0, request);
            var selectLockedDiamond = diamondListResponse
                .Where(x => x.Status == Domain.Common.Enums.ProductStatus.LockForUser && x.ProductLock != null)
                .ToList();
            var getAccounts = await _accountRepository.GetAccounts(selectLockedDiamond.Select(x => x.ProductLock.AccountId).ToList());   
            foreach (var diamond in selectLockedDiamond)
            {
                var accountLocked = getAccounts.FirstOrDefault(x => x.Id == diamond.ProductLock.AccountId);
                diamond.ProductLock.Account = accountLocked;
            }
            var totalPage = (int)Math.Ceiling((decimal)Count / (decimal)pageSize);
            var response = new PagingResponseDto<Diamond>(
                TotalPage: totalPage,
                CurrentPage: start,
                Values: finalResult,
                totalCount: Count,
                totalTake: finalResult.Count
                );
            return response;
        }//,List<DiamondPrice> roundPrice, List<DiamondPrice> fancyPrice
        private async Task<List<Diamond>> GetData(List<Diamond> responseList, int start, int size, int responseSkip, GetDiamondPagingQuery request)
        {
            var trueSkip = request.start * request.pageSize;
            var currentStart = start * size;
            //if (responseList.Count >= TotalTake)
            //{
            //    var countNow = responseList.Count;
            //    // phai tinh count rieng, khuc nay la khi response no chua dat toi max trong db
            //    // ==> count response list ko dung voi db ==> phair chay laij filter roi count
            //    Count = responseList.Count;
            //    var currentResult = responseList.Skip(trueSkip).Take(size).ToList();
            //    responseList = currentResult;

            //    return new List<Diamond>(currentResult);
            //}
            if (MaxInDb <= start * size)
            {
                Count = responseList.Count;
                var currentResult = responseList.Skip(trueSkip).Take(size).ToList();
                responseList = currentResult;
                return new List<Diamond>(currentResult);
            }
            DiamondShapeId? parsedShapeId = null;
            if(request.shapeId != null)
                parsedShapeId = DiamondShapeId.Parse(request.shapeId);
            var query = _diamondRepository.GetQuery();
            
            if (AccountRole.ShopRoles.Any(x => _httpContextAccessor.HttpContext.User.IsInRole(x.Id.Value)) is false)//not in shop
            {
                query = _diamondRepository.QueryFilter(query, d => d.Status == Domain.Common.Enums.ProductStatus.Active);
            }
            else
            {
                var managerQuery = request.GetDiamond_ManagerQuery;
                if (managerQuery != null)
                {
                    if(managerQuery.diamondStatuses is not null)
                        query = _diamondRepository.QueryStatus(query, managerQuery.diamondStatuses);
                }
            }
            if (IncludeJewelryDiamond is false)
                query = _diamondRepository.QueryFilter(query, d => d.JewelryId == null);
            if(request.isLab != null)
                query = _diamondRepository.QueryFilter(query, d => d.IsLabDiamond == request.isLab);
            if(parsedShapeId is not null)
                query = query.Where(d => d.DiamondShapeId == parsedShapeId);
            if (request.diamond_4C is not null)
                query = Filtering4C(query, request.diamond_4C);
            if (request.diamond_Details is not null)
                query = FilteringDetail(query, request.diamond_Details);
            query = query.Skip(currentStart);
            query = query.Take(size);
            List<Diamond> trueResult = new();
            var result = query.ToList();

            

            foreach (var diamond in result)
            {
                DiamondPrice diamondPrice;
                diamond.DiamondShape = Shapes.FirstOrDefault(s => s.Id == diamond.DiamondShapeId);
                var diamondPriceBySHape = await _diamondPriceRepository.GetPrice(diamond.Cut.Value, diamond.DiamondShape,diamond.IsLabDiamond);
                diamondPrice = await _diamondService.GetDiamondPrice(diamond, diamondPriceBySHape);
                _diamondService.AssignDiamondDiscount(diamond, ActiveDiscount).Wait();
            }
            trueResult = FilteringPrice(result, request.priceStart, request.priceEnd);
            //neu co sku. bo het query tren va tim theo sku
            if (request.GetDiamond_ManagerQuery is not null)
            {
                if (request.GetDiamond_ManagerQuery.sku is not null)
                {
                    var getResult = await _diamondRepository.GetWhereSkuContain(request.GetDiamond_ManagerQuery.sku, 0, TotalTake );
                    var trueResultSku = getResult.Skip(trueSkip).Take(request.pageSize).ToList();
                    responseList.AddRange(trueResult);
                    Count = getResult.Count;
                    foreach (var diamond in trueResultSku)
                    {
                        DiamondPrice diamondPrice;
                        diamond.DiamondShape = Shapes.FirstOrDefault(s => s.Id == diamond.DiamondShapeId);
                        var diamondPriceBySHape = await _diamondPriceRepository.GetPrice(diamond.Cut.Value, diamond.DiamondShape, diamond.IsLabDiamond);
                        diamondPrice = await _diamondService.GetDiamondPrice(diamond, diamondPriceBySHape);
                        _diamondService.AssignDiamondDiscount(diamond, ActiveDiscount).Wait();
                    }
                    return trueResultSku;
                }
            }
            responseList.AddRange(trueResult);
            //if(responseList.Count >= size)
            //{
            //    responseList = responseList.OrderByDescending(x => x.TruePrice).Take(size).ToList();
            //    return start;
            //}
            //else
            //{
            var responseTrueSkip = responseSkip + trueResult.Count;
            return await GetData(responseList, start + 1, size, responseTrueSkip, request);
            //}
        }
        private List<Diamond> FilteringPrice(List<Diamond> diamondWithPricesAssigned, decimal startPrice, decimal endPrice)
        {
            var result = new List<Diamond>();
            foreach (var diamond in diamondWithPricesAssigned)
            {
                if (diamond.TruePrice >= startPrice && diamond.TruePrice <= endPrice)
                    result.Add(diamond);
            }
            return result;
        }
        private IQueryable<Diamond> Filtering4C(IQueryable<Diamond> query, GetDiamond_4C diamond_4C)
        {
            if (diamond_4C.cutFrom is not null || diamond_4C.cutTo is not null)
                query = _diamondRepository.QueryFilter(query, d => d.Cut == null || (d.Cut >= diamond_4C.cutFrom && d.Cut <= diamond_4C.cutTo));
            if (diamond_4C.clarityFrom is not null || diamond_4C.clarityTo is not null)
                query = _diamondRepository.QueryFilter(query, d => d.Clarity >= diamond_4C.clarityFrom && d.Clarity <= diamond_4C.clarityTo);
            if (diamond_4C.colorFrom is not null || diamond_4C.colorTo is not null)
                query = _diamondRepository.QueryFilter(query, d => d.Color >= diamond_4C.colorFrom && d.Color <= diamond_4C.colorTo);
            if (diamond_4C.caratFrom is not null || diamond_4C.caratTo is not null)
                query = _diamondRepository.QueryFilter(query, d => d.Carat >= diamond_4C.caratFrom && d.Carat <= diamond_4C.caratTo);
            return query;
        }
        private IQueryable<Diamond> FilteringDetail(IQueryable<Diamond> query, GetDiamond_Details diamond_Details)
        {
            if (diamond_Details.Culet is not null)
                query = _diamondRepository.QueryFilter(query, d => d.Culet == diamond_Details.Culet);
            if (diamond_Details.Fluorescence is not null)
                query = _diamondRepository.QueryFilter(query, d => d.Fluorescence == diamond_Details.Fluorescence);
            if (diamond_Details.Polish is not null)
                query = _diamondRepository.QueryFilter(query, d => d.Polish == diamond_Details.Polish);
            if (diamond_Details.Girdle is not null)
                query = _diamondRepository.QueryFilter(query, d => d.Girdle == diamond_Details.Girdle);
            return query;
        }
    }
}

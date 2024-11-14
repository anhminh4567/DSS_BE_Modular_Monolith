using DiamondShop.Application.Commons.Responses;
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
        private List<Discount> ActiveDiscount = new();
        private List<DiamondShape> Shapes = new();
        private int Count = 0;
        private bool IncludeJewelryDiamond = false;
        private int TotalTake = 0;
        public GetDiamondPagingQueryHandler(IDiamondRepository diamondRepository, IDiamondPriceRepository diamondPriceRepository, IDiamondServices diamondService, IDiamondShapeRepository diamondShapeRepository, IDiscountService discountService, IDiscountRepository discountRepository, IHttpContextAccessor httpContextAccessor)
        {
            _diamondRepository = diamondRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondService = diamondService;
            _diamondShapeRepository = diamondShapeRepository;
            _discountService = discountService;
            _discountRepository = discountRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<PagingResponseDto<Diamond>>> Handle(GetDiamondPagingQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out bool isLab, out bool? includeJewelryDiamond , out int pageSize, out int start, out string shapeId, out decimal priceStart, out var priceEnd, out var diamond_4C, out var diamond_Details);
            var query = _diamondRepository.GetQuery();
            //List<DiamondPrice> getRoundPrice = new();
            //List<DiamondPrice> getFancyPrice = new();
            List<Diamond> diamondListResponse = new();
            IncludeJewelryDiamond = includeJewelryDiamond.Value;
            //getFancyPrice = await _diamondPriceRepository.GetPrice(true, request.isLab, cancellationToken);
            //getRoundPrice = await _diamondPriceRepository.GetPrice(false, request.isLab, cancellationToken);
            var getAllShape = await _diamondShapeRepository.GetAll();
            Shapes = getAllShape;
            var getActiveDiscount = await _discountRepository.GetActiveDiscount();
            ActiveDiscount = getActiveDiscount;
            if (AccountRole.ShopRoles.Any(x => _httpContextAccessor.HttpContext.User.IsInRole(x.Id.Value)) is false)//not in shop
            {
                query = _diamondRepository.QueryFilter(query, d => d.Status == Domain.Common.Enums.ProductStatus.Active);
            }
            var count = _diamondRepository.GetCount();
            Count = count;
            TotalTake = (start + 1) * pageSize;
            var finalResult = await GetData(diamondListResponse, 0, pageSize, request);
            //var parsedShape = DiamondShapeId.Parse(shapeId);
            //query = query.Where(d => d.DiamondShapeId == parsedShape);
            ////query = _diamondRepository.QueryInclude(query, d => d.DiamondShape);
            //if (diamond_4C is not null)
            //    query = Filtering4C(query, diamond_4C);
            //if (diamond_Details is not null)
            //    query = FilteringDetail(query, diamond_Details);
            //query.Skip(start * pageSize);
            //query.Take(pageSize);
            //var result = query.ToList();
            

            ////var getAllShape = await _diamondShapeRepository.GetAll();
            ////Dictionary<string, List<DiamondPrice>> shapeDictPrice = new();
            ////foreach (var shape in getAllShape)
            ////{
            ////    var prices = await _diamondPriceRepository.GetPriceByShapes(shape,null, cancellationToken);
            ////    shapeDictPrice.Add(shape.Id.Value,prices);
            ////}

            //var getAllDiscount = await _discountRepository.GetActiveDiscount();
            //foreach (var diamond in result)
            //{
            //    DiamondPrice diamondPrice;
            //    diamond.DiamondShape = getAllShape.FirstOrDefault(s => s.Id == diamond.DiamondShapeId);
            //    if (DiamondShape.IsFancyShape(diamond.DiamondShapeId))
            //        diamondPrice = await _diamondService.GetDiamondPrice(diamond, getFancyPrice);
            //    else
            //        diamondPrice = await _diamondService.GetDiamondPrice(diamond, getRoundPrice);
            //    //diamond.DiamondPrice = diamondPrice;
            //    _diamondService.AssignDiamondDiscount(diamond, getAllDiscount).Wait();
            //}

            //var totalPage = (int)Math.Ceiling((decimal)count / (decimal)pageSize);
            //var response = new PagingResponseDto<Diamond>(
            //    TotalPage: totalPage,
            //    CurrentPage: start,
            //    Values: result
            //    );
            var totalPage = (int)Math.Ceiling((decimal)count / (decimal)pageSize);
            var response = new PagingResponseDto<Diamond>(
                TotalPage: totalPage,
                CurrentPage: start,
                Values: diamondListResponse
                );
            return response;
        }//,List<DiamondPrice> roundPrice, List<DiamondPrice> fancyPrice
        private async Task<int> GetData(List<Diamond> responseList  , int start, int size, GetDiamondPagingQuery request )
        {
            var currentStart = start * size;
            if (responseList.Count >= TotalTake)
            {
                var countNow = responseList.Count;
                var currentResult = responseList.Skip(currentStart).Take(size).ToList();
                responseList = currentResult;
                return currentStart;
            }
            if (Count <= start * size) 
            {
                return -1;
            }
            var parsedShape = DiamondShapeId.Parse(request.shapeId);
            var query = _diamondRepository.GetQuery();
            if (AccountRole.ShopRoles.Any(x => _httpContextAccessor.HttpContext.User.IsInRole(x.Id.Value)) is false)//not in shop
            {
                query = _diamondRepository.QueryFilter(query, d => d.Status == Domain.Common.Enums.ProductStatus.Active);
                //if(IncludeJewelryDiamond is false)
                //    query = _diamondRepository.QueryFilter(query, d => d.JewelryId == null);
            }
            if (IncludeJewelryDiamond is false)
                query = _diamondRepository.QueryFilter(query, d => d.JewelryId == null);
            query = _diamondRepository.QueryFilter(query, d => d.IsLabDiamond == request.isLab);
            query = query.Where(d => d.DiamondShapeId == parsedShape);
            if (request.diamond_4C is not null)
                query = Filtering4C(query, request.diamond_4C);
            if (request.diamond_Details is not null)
                query = FilteringDetail(query, request.diamond_Details);
            query.Skip(currentStart);
            query.Take(size);
            var result = query.ToList();

            foreach (var diamond in result)
            {
                DiamondPrice diamondPrice;
                diamond.DiamondShape = Shapes.FirstOrDefault(s => s.Id == diamond.DiamondShapeId);
                //if (DiamondShape.IsFancyShape(diamond.DiamondShapeId))
                //    diamondPrice = await _diamondService.GetDiamondPrice(diamond, fancyPrice);
                //else
                //    diamondPrice = await _diamondService.GetDiamondPrice(diamond, roundPrice);
                var diamondPriceBySHape =await _diamondPriceRepository.GetPrice(diamond.Cut.Value,diamond.DiamondShape, null);
                diamondPrice = await _diamondService.GetDiamondPrice(diamond, diamondPriceBySHape);
                _diamondService.AssignDiamondDiscount(diamond, ActiveDiscount).Wait();
            }
            var trueResult = FilteringPrice(result,request.priceStart, request.priceEnd);
             responseList.AddRange(trueResult);
            if(responseList.Count >= size)
            {
                responseList = responseList.OrderByDescending(x => x.TruePrice).Take(size).ToList();
                return start;
            }
            else
            {//size - responseList.Count
                return await GetData(responseList, start + 1, size, request);
            }
        }
        private List<Diamond> FilteringPrice(List<Diamond> diamondWithPricesAssigned , decimal startPrice, decimal endPrice)
        {
            var result =  new List<Diamond>();
            foreach(var diamond in diamondWithPricesAssigned)
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

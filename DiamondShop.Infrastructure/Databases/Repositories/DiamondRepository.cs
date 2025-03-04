﻿using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using Microsoft.Extensions.Options;
using DiamondShop.Domain.Common;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using Syncfusion.XlsIO.Implementation.Security;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;


namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class DiamondRepository : BaseRepository<Diamond>, IDiamondRepository
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public DiamondRepository(DiamondShopDbContext dbContext, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor) : base(dbContext)
        {
            _optionsMonitor = optionsMonitor;
        }

        public void UpdateRange(List<Diamond> diamonds)
        {
            _set.UpdateRange(diamonds);
        }

        public override Task<Diamond?> GetById(params object[] ids)
        {
            DiamondId id = (DiamondId)ids[0];
            return _set.Include(d => d.DiamondShape).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<(Diamond diamond, List<Discount> discounts, List<Promotion> promotion)> GetByIdIncludeDiscountAndPromotion(DiamondId id, CancellationToken cancellationToken = default)
        {
            var result = await _set.Where(d => d.Id == id)
                .Select(diamond => new
                {
                    Diamond = diamond,
                    Discounts = _dbContext.Discounts
                    .Where(discount => discount.DiscountReq.Where(d => d.TargetType == TargetType.Diamond).Count() > 0).ToList(),
                    Promotions = _dbContext.Promotions.Where(promo => promo.PromoReqs
                    .Where(d => d.TargetType == TargetType.Diamond).Count() > 0).ToList(),
                }).FirstAsync() ;
            return ( diamond : result.Diamond, discounts : result.Discounts, prmotion : result.Promotions );
            //.FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<Diamond>> GetDiamondsJewelry(JewelryId jewelryId, CancellationToken cancellationToken = default)
        {
            return _set.Where(d => d.JewelryId == jewelryId).Include(d => d.DiamondShape).ToListAsync(cancellationToken);
        }

        public Task<List<Diamond>> GetAllAdmin(CancellationToken cancellationToken = default)
        {
            return _set.IgnoreQueryFilters().ToListAsync();
        }

        public Task<List<Diamond>> GetUserLockDiamonds(AccountId accountId, CancellationToken cancellationToken = default)
        {
            return _set.IgnoreQueryFilters().Where(x => x.ProductLock != null 
            && x.ProductLock.AccountId == accountId 
            && x.Status == ProductStatus.LockForUser)
                .ToListAsync(cancellationToken);
        }

        public Task<List<Diamond>> GetLockDiamonds(CancellationToken cancellationToken = default)
        {
            return _set.IgnoreQueryFilters().Where(x => x.ProductLock != null).ToListAsync();
        }

        public Task<List<Diamond>> GetBySkus(string[] skus, CancellationToken cancellationToken = default)
        {
            return _set.IgnoreQueryFilters().Where(x => skus.Contains(x.SerialCode)).ToListAsync(cancellationToken);
        }

        public Task<List<Diamond>> GetRange(List<DiamondId> diamondIds, CancellationToken cancellationToken = default)
        {
            return _set.Where(x => diamondIds.Contains(x.Id)).ToListAsync(cancellationToken);
        }

        public IQueryable<Diamond> QueryStatus(IQueryable<Diamond> query, List<ProductStatus> diamondStatusesToLookFor)
        {
            query = query.Where(x => diamondStatusesToLookFor.Contains(x.Status));
            return query;
        }

        public async Task<List<Diamond>> GetTotalSoldDiamonds(bool? isLab, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            List<Order> orders = new List<Order>();
            var orderQuery = _dbContext.Orders.AsQueryable()
                .Where(x => x.Status != OrderStatus.Cancelled
                && x.Status != OrderStatus.Rejected
                && x.CancelledDate == null);
            if(startDate != null)
                orderQuery = orderQuery.Where(x => x.CreatedDate >= startDate);
            if(endDate != null)
                orderQuery = orderQuery.Where(x => x.CreatedDate <= endDate);
            var query = orderQuery.Include(x => x.Items)
                .SelectMany(x => x.Items)
                .Where(x => x.DiamondId != null)
                    .Include(x => x.Diamond)
                    .Select(x => x.Diamond);
            if(isLab != null)
            {
                query = query.Where(x => x.IsLabDiamond == isLab.Value);
            }
            var soldDiamonds = await query.AsSplitQuery().ToListAsync();
            return soldDiamonds;
        }

        public async Task<List<Diamond>> GetTotalSoldDiamondsByShape(DiamondShape shape, bool? isLab, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            var orderQuery = _dbContext.Orders.AsQueryable()
                .Where(x => x.Status != OrderStatus.Cancelled
                && x.Status != OrderStatus.Rejected
                && x.CancelledDate == null);
            if (startDate != null)
                orderQuery = orderQuery.Where(x => x.CreatedDate >= startDate);
            if (endDate != null)
                orderQuery = orderQuery.Where(x => x.CreatedDate <= endDate);
            var query = orderQuery.Include(x => x.Items)
                .SelectMany(x => x.Items)
                .Where(x => x.DiamondId != null)
                    .Include(x => x.Diamond)
                    .Select(x => x.Diamond)
                        .Where(x => x.DiamondShapeId == shape.Id);
            if(isLab != null)
            {
                query = query.Where(x => x.IsLabDiamond == isLab.Value);
            }
            var soldDiamonds = await query.AsSplitQuery().ToListAsync();
            return soldDiamonds;
        }

        public Task<int> GetCountByStatus(List<ProductStatus> diamondStatusesToLookFor, bool? isLab, bool includeAttachingToJewelry = true)
        {
            var query = _set.IgnoreQueryFilters()
                .Where(x => diamondStatusesToLookFor.Contains(x.Status));
            if (includeAttachingToJewelry == false)
            {
                query = query.Where(x => x.JewelryId == null);
            }
            if (isLab != null)
            {
                query = query.Where(x => x.IsLabDiamond == isLab.Value);
            }
            return query.CountAsync();
        }

        public Task<int> GetCountByShapeAndStatus(List<ProductStatus> diamondStatusesToLookFor, bool? isLab, List<DiamondShapeId> shapesToLookFor, bool includeAttachingToJewelry = true)
        {
            var query = _set.IgnoreQueryFilters()
                .Where(x => diamondStatusesToLookFor.Contains(x.Status) && shapesToLookFor.Contains(x.DiamondShapeId));
            if (includeAttachingToJewelry == false)
            {
                query = query.Where(x => x.JewelryId == null);
            }
            if(isLab != null)
            {
                query = query.Where(x => x.IsLabDiamond == isLab.Value);
            }
            return query.CountAsync();
        }

        public Task<List<Diamond>> GetWhereSkuContain(string containingString, int skip, int take, CancellationToken cancellationToken = default)
        {
            return _set.Where(x => x.SerialCode.ToUpper().Contains(containingString.ToUpper())).Skip(skip).Take(take).ToListAsync(cancellationToken);
        }


        public Task ExecuteUpdateDiamondUpdatedTime(IQueryable<Diamond> query)
        {
            var dateTimeNow = DateTime.UtcNow;
            query.ExecuteUpdate(x => x.SetProperty(x => x.UpdatedAt,dateTimeNow ));
            return Task.CompletedTask;
        }

        public IQueryable<Diamond> Filtering4C(IQueryable<Diamond> query, GetDiamond_4C diamond_4C)
        {
            if (diamond_4C.cutFrom is not null || diamond_4C.cutTo is not null)
                query = query.Where(d => d.Cut == null || (d.Cut >= diamond_4C.cutFrom && d.Cut <= diamond_4C.cutTo));
            if (diamond_4C.clarityFrom is not null || diamond_4C.clarityTo is not null)
                query = query.Where(d => d.Clarity >= diamond_4C.clarityFrom && d.Clarity <= diamond_4C.clarityTo);
            if (diamond_4C.colorFrom is not null || diamond_4C.colorTo is not null)
                query = query.Where(d => d.Color >= diamond_4C.colorFrom && d.Color <= diamond_4C.colorTo);
            if (diamond_4C.caratFrom is not null || diamond_4C.caratTo is not null)
                query = query.Where(d => d.Carat >= diamond_4C.caratFrom && d.Carat <= diamond_4C.caratTo);
            return query;
        }
        public IQueryable<Diamond> FilteringDetail(IQueryable<Diamond> query, GetDiamond_Details diamond_Details)
        {
            if (diamond_Details == null)
                return query;
            if (diamond_Details.Culet is not null)
                query = query.Where(d => d.Culet == diamond_Details.Culet);
            if (diamond_Details.Fluorescence is not null)
                query = query.Where(d => d.Fluorescence == diamond_Details.Fluorescence);
            if (diamond_Details.Polish is not null)
                query = query.Where(d => d.Polish == diamond_Details.Polish);
            if (diamond_Details.Girdle is not null)
                query = query.Where(d => d.Girdle == diamond_Details.Girdle);
            return query;
        }

        public async Task<IQueryable<Diamond>> FilteringPrice(IQueryable<Diamond> query, GetDiamond_4C diamond_4C, decimal priceFrom, decimal priceTo)
        {
            var diamondRule = _optionsMonitor.CurrentValue.DiamondRule;
            var testId = DiamondCriteriaId.Parse("test");
            var smallestDiamondPriceAllowed = diamondRule.MinimalMainDiamondPrice;
            var joinDiamondCriteriaQuery = from diamond in query // Start with all diamonds
                                           join criteria in _dbContext.DiamondCriteria
                                                on new
                                                {
                                                    ShapeId = DiamondShape.All_Fancy_Shape.Select(x => x.Id).Contains(diamond.DiamondShapeId) // Check if diamond is a fancy shape
                                                        ? DiamondShape.FANCY_SHAPES.Id// If fancy, use the diamond's ShapeId
                                                        : diamond.DiamondShapeId  // For round shapes, join with the generic round criteria
                                                }
                                                equals new
                                                {
                                                    ShapeId = DiamondShape.All_Fancy_Shape.Select(x => x.Id).Contains(criteria.ShapeId) // Check if criteria matches fancy shapes
                                                        ? DiamondShape.FANCY_SHAPES.Id // Use the criteria's ShapeId for fancy
                                                        : criteria.ShapeId// Match round criteria for non-fancy shapes
                                                }
                                                into criteriaGroup
                                            from matchedCriteria in criteriaGroup.DefaultIfEmpty() // Include diamonds without matching criteria
                                            let isValidCriteriaJoin = (matchedCriteria != null
                                               && diamond.Carat >= matchedCriteria.CaratFrom // Carat validation
                                               && diamond.Carat <= matchedCriteria.CaratTo) || matchedCriteria == null
                                            select new
                                            {
                                                Diamond = diamond,
                                                IsValid = isValidCriteriaJoin,
                                                Criteria = matchedCriteria != null ? matchedCriteria.Id : null // Nullify invalid criteria
                                            };
            joinDiamondCriteriaQuery = joinDiamondCriteriaQuery.Where(x => x.IsValid);
            joinDiamondCriteriaQuery = joinDiamondCriteriaQuery.Distinct();
            var rejoinQuery = from diamond in query
                              join valid in joinDiamondCriteriaQuery
                                  on diamond.Id equals valid.Diamond.Id into validGroup
                              from validCriteria in validGroup.DefaultIfEmpty() // LEFT JOIN
                              select new
                              {
                                  Diamond = diamond,
                                  Criteria = validCriteria == null ? null : validCriteria.Criteria, // Null if no valid criteria
                                  diamond.Id
                              };
            //var queryString = rejoinQuery.ToQueryString();
            //var testResult = rejoinQuery.ToList();
            //var testingQuery = from joinedQuery in rejoinQuery
            //                   join price in _dbContext.DiamondPrices
            //                   on new
            //                   {
            //                       CriteriaId = joinedQuery.Criteria != null ? joinedQuery.Criteria : DiamondCriteriaId.Parse("-404"), // Match only if criteria exists
            //                       Color = joinedQuery.Diamond.Color,
            //                       Clarity = joinedQuery.Diamond.Clarity,
            //                       IsLab = joinedQuery.Diamond.IsLabDiamond
            //                   }
            //                   equals new
            //                   {
            //                       CriteriaId = price.CriteriaId,
            //                       Color = price.Color ?? 0,
            //                       Clarity = price.Clarity ?? 0,
            //                       IsLab = price.IsLabDiamond
            //                   }
            //                   into priceGroup
            //                   from matchedPrice in priceGroup.DefaultIfEmpty()
            //                   select new
            //                   {
            //                       joinedQuery.Diamond.Id,
            //                       priceId = matchedPrice != null ? matchedPrice.Id : null,
            //                       price = matchedPrice != null ? matchedPrice.Price : (decimal?)null,
            //                   };
            var joinPriceQuery = (from joinedQuery in rejoinQuery
                                  join price in _dbContext.DiamondPrices
                                  on new
                                  {
                                      CriteriaId = joinedQuery.Criteria != null ? joinedQuery.Criteria : DiamondCriteriaId.Parse("-404"), // Match only if criteria exists
                                      Color = joinedQuery.Diamond.Color,
                                      Clarity = joinedQuery.Diamond.Clarity,
                                      IsLab = joinedQuery.Diamond.IsLabDiamond,
                                      IsSideDiamond = false,
                                  }
                                  equals new
                                  {
                                      CriteriaId = price.CriteriaId,
                                      Color = price.Color ?? 0,
                                      Clarity = price.Clarity ?? 0,
                                      IsLab = price.IsLabDiamond,
                                      IsSideDiamond = false,
                                  }
                                  into priceGroup
                                  from matchedPrice in priceGroup.DefaultIfEmpty() // Include diamonds without matching prices
                                  let rawComputedPrice = matchedPrice != null
                                      ? matchedPrice.Price * (1m + joinedQuery.Diamond.PriceOffset) * (decimal)joinedQuery.Diamond.Carat + ( joinedQuery.Diamond.FixPriceOffset == null ? 0 : joinedQuery.Diamond.FixPriceOffset.Value) // Compute price
                                      : (decimal?)null // Compute price, or null if no match
                                  let roundedComputedPrice = rawComputedPrice != null
                                     ? Math.Ceiling(rawComputedPrice.Value / 1000m) * 1000 // Round up to the nearest 1000 VND
                                     : (decimal?)null
                                  let computedPrice = roundedComputedPrice == null
                                  ? (decimal?)null
                                  : (roundedComputedPrice <= smallestDiamondPriceAllowed)
                                     ? smallestDiamondPriceAllowed
                                     : roundedComputedPrice // Ensure price is at least the smallest allowed price
                                                            //where joinedQuery.Criteria == null // Ensure diamonds without criteria are included
                                                            //     || matchedPrice == null // Ensure diamonds without prices are included
                                                            //     || computedPrice == null // Ensure null computedPrice is included
                                                            //     || (computedPrice >= priceFrom && computedPrice <= priceTo) // Filter by price range // Filter by price range
                                  select new
                                  {
                                      joinedQuery.Diamond,
                                      ComputedPrice = computedPrice,
                                      Priority = computedPrice != null ? 1 : 0
                                  });//.Where(x => x.ComputedPrice == null || (x.ComputedPrice >= priceFrom && x.ComputedPrice <= priceTo));
            var finalQuery = joinPriceQuery.Where(x => x.ComputedPrice == null || (x.ComputedPrice >= priceFrom && x.ComputedPrice <= priceTo))
                .GroupBy(x => x.Diamond.Id)
                .Select(g => g.OrderByDescending(x => x.Priority).First().Diamond)// Select the highest priority row per diamond
                ;//.ToList(); 
            //var testquery = joinPriceQuery.Where(x => x.ComputedPrice == null || (x.ComputedPrice >= priceFrom && x.ComputedPrice <= priceTo))
            //    .GroupBy(x => x.Diamond.Id)
            //    .Select(g => g.OrderByDescending(x => x.Priority).Select(x => new { x.Diamond.Id, x.ComputedPrice })
            //    .Where(x => x.ComputedPrice >= priceFrom && x.ComputedPrice <= priceTo));
            //.Where(x => x.ComputedPrice == null || (x.ComputedPrice >= priceFrom && x.ComputedPrice <= priceTo));
            var testquery1 = joinPriceQuery.Where(x => x.ComputedPrice == null || (x.ComputedPrice >= priceFrom && x.ComputedPrice <= priceTo))
                .OrderBy(x => x.Diamond.Carat).ThenBy(x => x.ComputedPrice)
                .Select(x => x.Diamond);//.Distinct();
            //var testquery2 = joinPriceQuery.Where(x => x.ComputedPrice == null || (x.ComputedPrice >= priceFrom && x.ComputedPrice <= priceTo))
            //    .OrderBy(x => x.Diamond.Carat)//.ThenBy(x => x.ComputedPrice)
            //    .Select(x => new {x.Diamond.Carat}) .Distinct();
            //var testquery3 = joinPriceQuery.Where(x => x.ComputedPrice == null || (x.ComputedPrice >= priceFrom && x.ComputedPrice <= priceTo))
            //    .OrderBy(x => x.Diamond.Carat)//.ThenBy(x => x.ComputedPrice)
            //    .Select(x => new { x.Diamond.Carat });//.Distinct();
            //var testquery4 = joinPriceQuery.Where(x => x.ComputedPrice == null || (x.ComputedPrice >= priceFrom && x.ComputedPrice <= priceTo))
            //    .OrderBy(x => x.Diamond.Carat).ThenBy(x => x.ComputedPrice)
            //    .Select(x => new { x.Diamond.Carat });
            //var testResult = testquery4.Skip(5).Take(5);
            return testquery1;
            //return finalQuery;
        }

        public async Task<IQueryable<Diamond>> GetWhereSkuContain(IQueryable<Diamond> query,string containingString, CancellationToken cancellationToken = default)
        {
            return query.Where(x => x.SerialCode.ToUpper().Contains(containingString.ToUpper()));
        }

        public async Task RemoveDiamondFromAllDiamondRequest(Diamond diamond)
        {
            DbSet<DiamondRequest> dbset = _dbContext.Set<DiamondRequest>();
            var getDiamondRequest = dbset.Where(x => x.DiamondId == diamond.Id).ToList();
            if (getDiamondRequest.Count == 0)
                return;
            foreach (var item in getDiamondRequest)
            {
                item.DiamondId = null;
                dbset.Update(item);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}

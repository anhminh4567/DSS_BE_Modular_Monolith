using DiamondShop.Domain.Models.Diamonds;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq.Expressions;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using System.Diagnostics;
using static OpenQA.Selenium.PrintOptions;
using Microsoft.EntityFrameworkCore.Internal;
using Syncfusion.XlsIO.Implementation.Collections.Grouping;
using FluentValidation.Results;
using System.Text.RegularExpressions;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class DiamondRepository : BaseRepository<Diamond>, IDiamondRepository
    {
        public DiamondRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
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
            return _set.Where(x => x.SerialCode.Contains(containingString)).Skip(skip).Take(take).ToListAsync(cancellationToken);
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
            //var matchingCriteriaWithPricesQuery =
            //    from c in _dbContext.DiamondCriteria
            //    join p in _dbContext.DiamondPrices
            //        on c.Id equals p.CriteriaId into priceGroup
            //    from p in priceGroup.DefaultIfEmpty()
            //    where c.IsSideDiamond == false
            //        && diamond_4C.caratFrom.Value <= c.CaratTo
            //        && c.CaratFrom <= diamond_4C.caratTo.Value
            //        && p.Color >= diamond_4C.colorFrom
            //         && (p == null || ( // Handle criteria without prices
            //           p.Color >= diamond_4C.colorFrom
            //           && p.Color <= diamond_4C.colorTo
            //           && p.Clarity >= diamond_4C.clarityFrom
            //           && p.Clarity <= diamond_4C.clarityTo))
            //    select new
            //    {
            //        Criteria = c,
            //        Price = p
            //    };
            //var diamondResult = await query.ToListAsync();

            var resultQuery =
                 from diamond in query // Start with all diamonds
                 join criteria in _dbContext.DiamondCriteria
                     on diamond.DiamondShapeId equals criteria.ShapeId // Match ShapeId
                     into criteriaGroup
                 from matchedCriteria in criteriaGroup.DefaultIfEmpty() // Include diamonds without criteria
                 where matchedCriteria == null // Include diamonds without matching criteria
                       || (matchedCriteria.IsSideDiamond == false // Ensure IsSideDiamond == false
                           && diamond.Carat >= matchedCriteria.CaratFrom // Carat within range
                           && diamond.Carat <= matchedCriteria.CaratTo
                           )
                 join price in _dbContext.DiamondPrices
                     on new
                     {
                         CriteriaId = matchedCriteria != null ? matchedCriteria.Id : DiamondCriteriaId.Parse("0"), // Match only if criteria exists
                         Color = diamond.Color,
                         Clarity = diamond.Clarity
                     }
                     equals new
                     {
                         CriteriaId = price.CriteriaId,
                         Color = price.Color ?? 0,
                         Clarity = price.Clarity ?? 0
                     }
                     into priceGroup
                 from matchedPrice in priceGroup.DefaultIfEmpty() // Include diamonds without matching prices
                 let computedPrice = matchedPrice != null
                 ? matchedPrice.Price * (1m + diamond.PriceOffset) * (decimal)diamond.Carat
                 : (decimal?)null // Compute price, or null if no match
                 where computedPrice == null || (computedPrice >= priceFrom && computedPrice <= priceTo) // Filter by price range
                 select new 
                 {
                     Diamond = diamond
                 };
            var testquery = resultQuery.Distinct().Select(x => x.Diamond);
            return testquery;
        }

        public async Task<IQueryable<Diamond>> GetWhereSkuContain(IQueryable<Diamond> query,string containingString, CancellationToken cancellationToken = default)
        {
            return query.Where(x => x.SerialCode.Contains(containingString));
        }
    }
}

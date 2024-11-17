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
            && x.IsLabDiamond 
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

        public async Task<List<Diamond>> GetTotalSoldDiamonds(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
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
            var soldDiamonds = await orderQuery.Include(x => x.Items)
                .SelectMany(x => x.Items)
                .Where(x => x.DiamondId != null)
                    .Include(x => x.Diamond)
                    .Select(x => x.Diamond)
                    .AsSplitQuery()
                .ToListAsync();
            return soldDiamonds;
        }

        public async Task<List<Diamond>> GetTotalSoldDiamondsByShape(DiamondShape shape, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            var orderQuery = _dbContext.Orders.AsQueryable()
                .Where(x => x.Status != OrderStatus.Cancelled
                && x.Status != OrderStatus.Rejected
                && x.CancelledDate == null);
            if (startDate != null)
                orderQuery = orderQuery.Where(x => x.CreatedDate >= startDate);
            if (endDate != null)
                orderQuery = orderQuery.Where(x => x.CreatedDate <= endDate);
            var soldDiamonds = await orderQuery.Include(x => x.Items)
                .SelectMany(x => x.Items)
                .Where(x => x.DiamondId != null)
                    .Include(x => x.Diamond)
                    .Select(x => x.Diamond)
                        .Where(x => x.DiamondShapeId == shape.Id)
                        .AsSplitQuery()
                .ToListAsync();
            return soldDiamonds;
        }

        public Task<int> GetCountByStatus(List<ProductStatus> diamondStatusesToLookFor, bool includeAttachingToJewelry = true)
        {
            var query = _set.IgnoreQueryFilters()
                .Where(x => diamondStatusesToLookFor.Contains(x.Status));
            if (includeAttachingToJewelry == false)
            {
                query = query.Where(x => x.JewelryId == null);
            }
            return query.CountAsync();
        }

        public Task<int> GetCountByShapeAndStatus(List<ProductStatus> diamondStatusesToLookFor, List<DiamondShapeId> shapesToLookFor, bool includeAttachingToJewelry = true)
        {
            var query = _set.IgnoreQueryFilters()
                .Where(x => diamondStatusesToLookFor.Contains(x.Status) && shapesToLookFor.Contains(x.DiamondShapeId));
            if (includeAttachingToJewelry == false)
            {
                query = query.Where(x => x.JewelryId == null);
            }
            return query.CountAsync();
        }
    }
}

using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.PromotionsRepo
{
    internal class PromotionRepository : BaseRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }
        public override Task<Promotion?> GetById(params object[] ids)
        {
            //var parsedId = PromotionId.Parse((string)ids[0]);
            return _set.Include(p => p.PromoReqs).ThenInclude(x => x.PromoReqShapes).Include(p => p.Gifts)
                .FirstOrDefaultAsync(p => p.Id == ids[0]);
        }
        public Task<List<Promotion>> GetActivePromotion(bool isDateComparisonRequired = false, CancellationToken cancellationToken = default)
        {
            if(isDateComparisonRequired) 
            {
                var now = DateTime.UtcNow;
                return _set.Include(p => p.PromoReqs).ThenInclude(x => x.PromoReqShapes).Include(p => p.Gifts)
                    .Where(p => p.Status == Domain.Models.Promotions.Enum.Status.Active && p.StartDate < now && p.EndDate > now)
                    .OrderBy(p => p.Priority)
                    .ToListAsync();
            }
            return _set.Include(p => p.PromoReqs).ThenInclude(x => x.PromoReqShapes).Include(p => p.Gifts)
                .Where(p => p.Status == Domain.Models.Promotions.Enum.Status.Active)
                .OrderBy(p => p.Priority)
                .ToListAsync();
        }

        public  Task<List<Order>> GetUserOrderThatUsedThisPromotion(Promotion promotion, Account userAccount, CancellationToken cancellationToken = default)
        {
            return _dbContext.Orders
                .Where(o => o.AccountId == userAccount.Id && o.PromotionId == promotion.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<Promotion?> GetByCode(string promotionCode, CancellationToken cancellationToken = default)
        {
            return _set.Where(x => x.PromoCode == promotionCode)
                .Include(x => x.PromoReqs)
                    .ThenInclude(x => x.PromoReqShapes)
                .Include(x => x.Gifts)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public IQueryable<Promotion> QueryByStatuses(IQueryable<Promotion> query, List<Status> statuses)
        {
           return query.Where(p => statuses.Contains(p.Status));
        }

        public Task<List<Promotion>> GetContainingCode(string code, int start, int take, CancellationToken cancellationToken = default)
        {
            return _set.Where(x => x.PromoCode.ToUpper().Contains(code.ToUpper())).Skip(start).Take(take).ToListAsync(cancellationToken);
        }

        public Task<int> GetPromotionCountFromOrders(Expression<Func<Order, bool>> expression)
        {
            return _dbContext.Orders.Where(expression).CountAsync();
        }

        public async Task<List<PromotionId>> GetPromotionIdsFromOrders(Expression<Func<Order, bool>> expression)
        {
            var getResult =await _dbContext.Orders
                .Where(x => x.PromotionId!= null && x.PromotionCode !=null)
                .Where(expression)
                .Select(x => x.PromotionId).ToListAsync();
            return getResult;
        }
    }
}

using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.PromotionsRepo
{
    internal class PromotionRepository : BaseRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public Task<List<Promotion>> GetActivePromotion(bool isDateComparisonRequired = false, CancellationToken cancellationToken = default)
        {
            if(isDateComparisonRequired) 
            {
                var now = DateTime.UtcNow;
                return _set.Include(p => p.PromoReqs).Include(p => p.Gifts).Where(p => p.Status == Domain.Models.Promotions.Enum.Status.Active && p.StartDate < now && p.EndDate > now).ToListAsync();
            }
            return _set.Include(p => p.PromoReqs).Include(p => p.Gifts).Where(p => p.Status == Domain.Models.Promotions.Enum.Status.Active).ToListAsync();
        }

        public Task<List<Order>> GetUserOrderThatUsedThisPromotion(Promotion promotion, Account userAccount, CancellationToken cancellationToken = default)
        {
            return _dbContext.Orders.Where(o => o.AccountId == userAccount.Id && o.PromotionId == promotion.Id).ToListAsync(cancellationToken);
        }
    }
}

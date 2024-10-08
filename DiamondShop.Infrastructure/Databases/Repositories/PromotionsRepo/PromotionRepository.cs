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
                return _set.Where(p => p.IsActive && p.StartDate < now && p.EndDate > now).ToListAsync();
            }
            return _set.Where(p => p.IsActive).ToListAsync();
        }
    }
}

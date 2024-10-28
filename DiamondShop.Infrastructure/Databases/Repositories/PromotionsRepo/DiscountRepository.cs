using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.PromotionsRepo
{
    internal class DiscountRepository : BaseRepository<Discount>, IDiscountRepository
    {
        public DiscountRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public Task<List<Discount>> GetActiveDiscount(bool isDateComparisonRequired = false,CancellationToken cancellationToken = default)
        {
            if(isDateComparisonRequired)
            {
                var now = DateTime.UtcNow;
                return _set.Include(d => d.DiscountReq)
                    .Where(d => d.Status == Domain.Models.Promotions.Enum.Status.Active && d.StartDate < now && d.EndDate > now)
                    .OrderByDescending(d => d.DiscountPercent)
                    .ToListAsync();
            }
            return _set.Include(d => d.DiscountReq)
                .Where(d => d.Status == Domain.Models.Promotions.Enum.Status.Active)
                .OrderByDescending(d => d.DiscountPercent)
                .ToListAsync();
   
        }

        public override Task<Discount?> GetById(params object[] ids)
        {
            return _set.Include(d => d.DiscountReq).FirstOrDefaultAsync(d => d.Id == (DiscountId)ids[0]);
        }
    }
}

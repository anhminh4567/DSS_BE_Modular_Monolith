using DiamondShop.Domain.Models.Promotions.Entities;
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
                return _set.Include(d => d.DiscountReq).ThenInclude(x => x.PromoReqShapes)
                    .Where(d => d.Status == Domain.Models.Promotions.Enum.Status.Active && d.StartDate < now && d.EndDate > now)
                    .OrderByDescending(d => d.DiscountPercent)
                    .ToListAsync();
            }
            return _set.Include(d => d.DiscountReq).ThenInclude(x => x.PromoReqShapes)
                .Where(d => d.Status == Domain.Models.Promotions.Enum.Status.Active)
                .OrderByDescending(d => d.DiscountPercent)
                .ToListAsync();
   
        }

        public Task<Discount?> GetByCode(string discountCode, CancellationToken cancellationToken = default)
        {
            return _set.Include(d => d.DiscountReq)
                .ThenInclude(x => x.PromoReqShapes)
                .FirstOrDefaultAsync(d => d.DiscountCode == discountCode, cancellationToken);
        }

        public override Task<Discount?> GetById(params object[] ids)
        {
            return _set.Include(d => d.DiscountReq)
                .ThenInclude(x => x.PromoReqShapes)
                .FirstOrDefaultAsync(d => d.Id == (DiscountId)ids[0]);
        }

        public Task<List<Discount>> GetContainingCode(string code, int start, int take, CancellationToken cancellationToken = default)
        {
            return _set.Where(x => x.DiscountCode.ToUpper().Contains(code.ToUpper())).Skip(start).Take(take).ToListAsync(cancellationToken);
        }

        public Task<int> GetDiscountCount(Expression<Func<Discount, bool>> expression)
        {
            return _set.Where(expression).CountAsync();
        }

        public Task<List<DiscountId>> GetDiscountIds(Expression<Func<Discount, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Discount> QueryByStatuses(IQueryable<Discount> query, List<Status> statuses)
        {
            return query.Where(d => statuses.Contains(d.Status));
        }
    }
}

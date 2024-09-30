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

        public override Task<Discount?> GetById(params object[] ids)
        {
            return _set.Include(d => d.DiscountReq).FirstOrDefaultAsync(d => d.Id == (DiscountId)ids[0]);
        }
    }
}

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
            return _set.Where(d => d.JewelryId == jewelryId).ToListAsync(cancellationToken);
        }
    }
}

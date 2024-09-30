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
    internal class GiftRepository : BaseRepository<Gift>, IGiftRepository
    {
        public GiftRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public Task CreateRange(List<Gift> gifts, CancellationToken cancellationToken = default)
        {
            return _set.AddRangeAsync(gifts, cancellationToken);
        }

        public Task<List<Gift>> GetRange(List<GiftId> ids, CancellationToken cancellationToken = default)
        {
            return _set.Where(g => ids.Contains(g.Id)).ToListAsync(cancellationToken);
        }
    }
}

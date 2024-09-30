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
    internal class RequirementRepository : BaseRepository<PromoReq>, IRequirementRepository
    {
        public RequirementRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public Task CreateRange(List<PromoReq> promoReqs, CancellationToken cancellationToken = default)
        {
            return _set.AddRangeAsync(promoReqs,cancellationToken);
        }

        public Task<List<PromoReq>> GetRange(List<PromoReqId> ids, CancellationToken cancellationToken = default)
        {
            return _set.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
        }
    }
}

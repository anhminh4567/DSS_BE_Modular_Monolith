using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.PromotionsRepo
{
    public interface IRequirementRepository : IBaseRepository<PromoReq>
    {
        Task CreateRange(List<PromoReq> promoReqs, CancellationToken cancellationToken = default);
        Task<List<PromoReq>> GetRange(List<PromoReqId> ids, CancellationToken cancellationToken = default);
    }
}

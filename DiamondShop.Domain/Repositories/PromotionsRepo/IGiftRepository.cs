using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.PromotionsRepo
{
    public interface IGiftRepository : IBaseRepository<Gift>
    {
        Task CreateRange(List<Gift> gifts, CancellationToken cancellationToken = default);
        Task<List<Gift>> GetRange(List<GiftId> ids, CancellationToken cancellationToken = default); 
    }
}

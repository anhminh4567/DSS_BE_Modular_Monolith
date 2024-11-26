using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.PromotionsRepo
{
    public interface IPromotionRepository : IBaseRepository<Promotion>
    {
        Task<List<Promotion>> GetActivePromotion(bool isDateComparisonRequired = false, CancellationToken cancellationToken = default);
        Task<List<Order>> GetUserOrderThatUsedThisPromotion(Promotion promotion, Account userAccount, CancellationToken cancellationToken = default);
        Task<Promotion?> GetByCode(string promotionCode, CancellationToken cancellationToken = default);
        IQueryable<Promotion> QueryByStatuses(IQueryable<Promotion> query, List<Status> statuses);
        Task<List<Promotion>> GetContainingCode(string code, int start, int take, CancellationToken cancellationToken = default);
    }
}

using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.PromotionsRepo
{
    public interface IDiscountRepository : IBaseRepository<Discount>
    {
        Task<List<Discount>> GetActiveDiscount(bool isDateComparisonRequired = false,CancellationToken cancellationToken =default);
        Task<Discount?> GetByCode(string discountCode, CancellationToken cancellationToken = default);
        IQueryable<Discount> QueryByStatuses(IQueryable<Discount> query,List<Status> statuses);
        Task<List<Discount>> GetContainingCode(string code, int start, int take, CancellationToken cancellationToken = default);
    }
}

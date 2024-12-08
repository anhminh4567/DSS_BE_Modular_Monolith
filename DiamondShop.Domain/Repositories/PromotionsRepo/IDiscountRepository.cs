using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        Task<int> GetDiscountCountFromOrder(Discount discount,Expression<Func<Order, bool>> expression);
        Task<decimal> GetDiscounMoneySpentOnOrders(Discount discount, Expression<Func<Order, bool>> expression);
        Task<List<OrderId>> GetOrderIdsFromOrder(Discount discount,Expression<Func<Order, bool>> expression);

        Task<List<DiscountId>> GetDiscountIds( Expression<Func<Discount, bool>> expression);
    }
}

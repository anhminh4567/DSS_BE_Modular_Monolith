using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Notifications;
using DiamondShop.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories
{
    public interface INotificationRepository : IBaseRepository<Notification> 
    {
        Task<List<Notification>> GetForOrder(Order order);
        Task<List<Notification>> GetForUser(Account account);
        Task<List<Notification>> GetPublicNotification(int start, int take);
        Task<int> DeleteBulk(Expression<Func<Notification, bool>> predicate);
    }
}

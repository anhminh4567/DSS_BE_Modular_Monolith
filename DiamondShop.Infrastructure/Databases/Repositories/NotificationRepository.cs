using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Notifications;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class NotificationRepository : BaseRepository<Notification>, INofificationRepository
    {
        public NotificationRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> DeleteBulk(Expression<Func<Notification, bool>> predicate)
        {
            return _set.Where(predicate).ExecuteDelete();
            
        }

        public Task<List<Notification>> GetForOrder(Order order)
        {
            return _set.Where(x => x.OrderId == order.Id).ToListAsync();
        }

        public Task<List<Notification>> GetForUser(Account account)
        {
            return _set.Where(x => x.AccountId == account.Id).ToListAsync();
        }

        public Task<List<Notification>> GetPublicNotification(int start, int take)
        {
            return _set.Where(x => x.AccountId == null && x.OrderId == null).Skip(start).Take(take).ToListAsync();
        }
    }
}

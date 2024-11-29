using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using DiamondShop.Domain.Repositories.TransactionRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.TransactionRepo
{
    internal class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }
        public override Task<Transaction?> GetById(params object[] ids)
        {
            var id = (TransactionId) ids[0];
            return _set.Include(p => p.Order).Include(x => x.PayMethod).FirstOrDefaultAsync(p => p.Id == id);
        }
        public Task<Transaction?> GetByAppAndPaygateId(string appid, string paygateId, CancellationToken cancellationToken = default)
        {
            return _set.FirstOrDefaultAsync(x => x.AppTransactionCode == appid && x.PaygateTransactionCode == paygateId, cancellationToken);
        }

        public Task<List<Transaction>> GetByOrderId(OrderId orderId, CancellationToken cancellationToken = default)
        {
            return _set.Where(x => x.OrderId == orderId).ToListAsync(cancellationToken);
        }
        public Task<bool> CheckExist(OrderId orderId, bool isManual, TransactionType transactionType)
        {
            return _set.AnyAsync(p => p.OrderId == orderId && p.IsManual == isManual && p.TransactionType == transactionType);
        }
    }
}

using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
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

        public Task<Transaction?> GetByAppAndPaygateId(string appid, string paygateId, CancellationToken cancellationToken = default)
        {
            return _set.FirstOrDefaultAsync(x => x.AppTransactionCode == appid && x.PaygateTransactionCode == paygateId, cancellationToken);
        }

        public Task<List<Transaction>> GetByOrderId(OrderId orderId, CancellationToken cancellationToken = default)
        {
            return _set.Where(x => x.OrderId == orderId).ToListAsync(cancellationToken);
        }
    }
}

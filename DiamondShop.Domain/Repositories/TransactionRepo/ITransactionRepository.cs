using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.TransactionRepo
{
    public interface ITransactionRepository : IBaseRepository<Transaction>
    {
        Task<Transaction?> GetByAppAndPaygateId(string appid, string paygateId, CancellationToken cancellationToken = default);
        Task<List<Transaction>> GetByOrderId(OrderId orderId, CancellationToken cancellationToken = default);
    }
}

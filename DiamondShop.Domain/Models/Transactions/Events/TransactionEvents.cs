using DiamondShop.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Transactions.Events
{
    public record TransactionCreatedEvent(Transaction Transaction, DateTime CreateDate) : IDomainEvent;
}

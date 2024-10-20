using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.AccountAggregate.Events
{
    public record CustomerCreatedMessage(AccountId AccountId, DateTime createdTimeUtc) : IDomainEvent;
}

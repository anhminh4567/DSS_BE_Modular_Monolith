using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Events
{
    public record OrderCompleteEvent(AccountId AccountId, OrderId OrderId, DateTime CompleteTime) :  IDomainEvent;
    public record OrderDonePayAllEvent(OrderId OrderId, DateTime PayTime) : IDomainEvent;
}

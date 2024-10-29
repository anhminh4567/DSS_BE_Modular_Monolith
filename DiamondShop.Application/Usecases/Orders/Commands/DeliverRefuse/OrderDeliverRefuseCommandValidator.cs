using DiamondShop.Application.Dtos.Requests.Carts;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using FluentValidation;
using MediatR;
using Newtonsoft.Json.Linq;

namespace DiamondShop.Application.Usecases.Orders.Commands.DeliverComplete
{
   public class OrderDeliverRefuseCommandValidator : AbstractValidator<OrderDeliverRefuseCommand>
    {
        public OrderDeliverRefuseCommandValidator()
        {
            RuleFor(p => p.OrderItemRefuseCommand).ChildRules(p =>
            {
                p.RuleForEach(b => b.Items).Must(CheckValid);
            });
        }
        bool CheckValid(OrderItemActionRequestDto item)
        {
            if (item != null && item.ItemId != null)
            {
                if ((item.Action == CompleteAction.ReplaceByShop || item.Action == CompleteAction.ReplaceByCustomer) && item.ReplacingItem != null)
                    return true;
            }
            return false;
        }
    }
}

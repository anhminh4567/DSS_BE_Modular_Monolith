using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;
using Newtonsoft.Json.Linq;

namespace DiamondShop.Application.Usecases.Orders.Commands.DeliverComplete
{
    public record OrderItemRefuseCommand(string OrderId, List<OrderItemActionRequestDto> Items);
    public record OrderDeliverRefuseCommand(string DelivererId, OrderItemRefuseCommand OrderItemRefuseCommand) : IRequest<Result<Order>>;
    internal class OrderDeliverRefuseCommandHandler : IRequestHandler<OrderDeliverRefuseCommand, Result<Order>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderDeliverRefuseCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderItemRepository orderItemRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<Result<Order>> Handle(OrderDeliverRefuseCommand request, CancellationToken token)
        {
            request.Deconstruct(out string delivererId, out OrderItemRefuseCommand orderItemRefuseCommand);
            orderItemRefuseCommand.Deconstruct(out string orderId, out List<OrderItemActionRequestDto> items);
            await _unitOfWork.BeginTransactionAsync(token);
            var orderQuery = _orderRepository.GetQuery();
            orderQuery = _orderRepository.QueryFilter(orderQuery, p => p.Id == OrderId.Parse(orderId));
            orderQuery = _orderRepository.GetDetailQuery(orderQuery);
            var order = orderQuery.FirstOrDefault();
            if (order == null)
                return Result.Fail("This order doesn't exist");
            if (order.DelivererId != AccountId.Parse(delivererId))
                return Result.Fail("Only the deliverer of this order can change its status.");
            //TODO: Complete refuse actions
            List<IError> errors = new List<IError>();
            foreach (var item in items)
            {
                var orderItem = order.Items.FirstOrDefault(p => p.Id == OrderItemId.Parse(item.ItemId));
                if (orderItem == null)
                {
                    errors.Add(new Error($"Item #{item.ItemId} doesn't exist"));
                    continue;
                }
                if (item.Action == CompleteAction.Refund)
                {
                    order.PaymentStatus = PaymentStatus.Refunding;
                    orderItem.Status = OrderItemStatus.Removed;
                }
                else
                {
                    orderItem.Status = OrderItemStatus.Replaced;
                    if (item.Action == CompleteAction.ReplaceByShop)
                    {

                    }
                    //ReplaceByCustomer
                    else
                    {
                    }
                }
            }
            _orderItemRepository.UpdateRange(order.Items);
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}

using DiamondShop.Application.Dtos.Requests.Carts;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson;
using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
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
        private readonly IOrderService _orderService;
        private readonly ISender _sender;

        public OrderDeliverRefuseCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderItemRepository orderItemRepository, IOrderService orderService, ISender sender)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderItemRepository = orderItemRepository;
            _orderService = orderService;
            _sender = sender;
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
            List<OrderItemRequestDto> newItemList = new();
            decimal totalRefund = 0m, oldPrice = 0m, newPrice = 0m;
            foreach (var item in items)
            {
                var orderItem = order.Items.FirstOrDefault(p => p.Id == OrderItemId.Parse(item.ItemId));
                var replacingItem = item.ReplacingItem;
                if (orderItem == null)
                {
                    errors.Add(new Error($"Item #{item.ItemId} doesn't exist"));
                    continue;
                }
                //Refund
                if (item.Action == CompleteAction.Refund)
                {
                    order.PaymentStatus = PaymentStatus.Refunding;
                    orderItem.Status = OrderItemStatus.Removed;
                    totalRefund += orderItem.PurchasedPrice;
                }
                //ReplaceByShop
                else if (item.Action == CompleteAction.ReplaceByShop)
                {
                    orderItem.Status = OrderItemStatus.Removed;
                    //Deactivate item jewelry or diamond immediately
                    SetActive(orderItem, false);
                    //Free of charge
                    newItemList.Add(item.ReplacingItem);       
                }
                //ReplaceByCustomer
                else if (item.Action == CompleteAction.ReplaceByCustomer)
                {
                    orderItem.Status = OrderItemStatus.Replaced;
                    SetActive(orderItem, true);
                    newItemList.Add(item.ReplacingItem);
                }
                //Complete
                else
                {

                }
            }
            if (errors.Count > 0)
                return Result.Fail(errors);
            else
                errors = new List<IError>();

            _orderItemRepository.UpdateRange(order.Items);
            
            //Create replacement order
            var newOrderInfo = new CreateOrderInfo(order.PaymentType, "zalopay", order.PromotionId.Value, order.ShippingAddress,newItemList);
            var orderResult = await _sender.Send(new CreateOrderCommand(order.AccountId.Value, newOrderInfo, order.Id.Value));
            if (orderResult.IsFailed)
                return Result.Fail(orderResult.Errors);
            await _orderRepository.Update(order);
            //TODO: Add payment 
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
        void SetActive(OrderItem item, bool active)
        {
            if (item.Diamond != null)
            {
                if (active)
                    item.Diamond.SetSell();
                else
                    item.Diamond.SetDeactivate();
            }
            else if (item.Jewelry != null)
            {
                if (active)
                {
                    item.Jewelry.SetSell();
                    item.Jewelry.Diamonds.ForEach(p => p.SetSell());
                }
                else
                {
                    item.Jewelry.SetDeactivate();
                    item.Jewelry.Diamonds.ForEach(p => p.SetDeactivate());
                }
            }
        }
    }
}

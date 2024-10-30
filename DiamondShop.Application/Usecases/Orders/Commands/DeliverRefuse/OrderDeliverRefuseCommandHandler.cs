using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Orders.Commands.DeliverComplete
{
    public record OrderItemRefuseCommand(string OrderId, List<OrderItemActionRequestDto> Items);
    public record OrderDeliverRefuseCommand(string DelivererId, OrderItemRefuseCommand OrderItemRefuseCommand) : IRequest<Result<Order>>;
    internal class OrderDeliverRefuseCommandHandler : IRequestHandler<OrderDeliverRefuseCommand, Result<Order>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly ISender _sender;

        public OrderDeliverRefuseCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderItemRepository orderItemRepository, IOrderService orderService, ISender sender, ITransactionRepository transactionRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderItemRepository = orderItemRepository;
            _orderService = orderService;
            _sender = sender;
            _transactionRepository = transactionRepository;
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
            List<OrderItem> oldItemList = new();
            decimal totalRefund = 0m, totalFine = 0m;
            foreach (var item in items)
            {
                var orderItem = order.Items.FirstOrDefault(p => p.Id == OrderItemId.Parse(item.ItemId));
                var replacingItem = item.ReplacingItem;
                if (orderItem == null)
                {
                    errors.Add(new Error($"Item #{item.ItemId} doesn't exist"));
                    continue;
                }
                //RefundByShop
                if (item.Action == CompleteAction.RefundByShop)
                {
                    orderItem.Status = OrderItemStatus.Removed;
                    totalRefund += orderItem.PurchasedPrice;
                    oldItemList.Add(orderItem);
                    //if PaidAll then refund 100%
                    decimal refund = 0m;
                    if (order.PaymentType == PaymentType.Payall)
                        refund = orderItem.PurchasedPrice;
                    //if COD then only refund the 10% since the order is not paid yet
                    else
                        refund += orderItem.PurchasedPrice * OrderPaymentRules.CODPercent;
                    totalRefund += refund;
                    totalFine += orderItem.PurchasedPrice - refund;
                }
                //RefundByCustomer
                if (item.Action == CompleteAction.RefundByCustomer)
                {
                    orderItem.Status = OrderItemStatus.Removed;
                    totalRefund += orderItem.PurchasedPrice;
                    oldItemList.Add(orderItem);
                    decimal refund = 0m;
                    //Customer's fault. Only paid 90% back
                    if (order.PaymentType == PaymentType.Payall)
                        refund = orderItem.PurchasedPrice * (1 - OrderPaymentRules.PayAllFine);
                    //if COD then no refund since we already take COD deposit
                    totalRefund += refund;
                    totalFine += orderItem.PurchasedPrice - refund;
                }
                //Complete
                else if (item.Action == CompleteAction.Complete)
                {
                    orderItem.Status = OrderItemStatus.Done;
                    oldItemList.Add(orderItem);
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
            }
            if (errors.Count > 0)
                return Result.Fail(errors);
            else
                errors = new List<IError>();
            //Create replacement order
            var newOrderInfo = new CreateOrderInfo(order.PaymentType, "zalopay", order.PromotionId?.Value, order.ShippingAddress, newItemList);
            var orderResult = await _sender.Send(new CreateOrderCommand(order.AccountId.Value, newOrderInfo, order, oldItemList));
            if (orderResult.IsFailed)
                return Result.Fail(orderResult.Errors);
            var newOrder = orderResult.Value;
            // if refund > 0 then allow refund
            if (totalRefund > 0)
            {
                order.PaymentStatus = PaymentStatus.Refunding;
            }
            var transferedAmount = order.Transactions.Where(p => p.TransactionType == Domain.Models.Transactions.Enum.TransactionType.Pay).Sum(p => p.TotalAmount);
            var refundTransac = Transaction.CreateManualRefund(order.Id, $"Replaced item refund for Order#{order.Id.Value}", transferedAmount - totalFine);
            await _transactionRepository.Create(refundTransac);
            await _orderRepository.Update(order);

            var newTransac = Transaction.CreateManualPayment(newOrder.Id, $"Replacement order#{newOrder.Id.Value} for order#{order.Id.Value}", _orderTransactionService.GetFullPaymentValueForOrder(order), Domain.Models.Transactions.Enum.TransactionType.Pay);
            await _transactionRepository.Create(newTransac);

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

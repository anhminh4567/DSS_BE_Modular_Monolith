using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Commands.Refund
{
    public record RefundOrderCommand(string orderId) : IRequest<Result<Order>>;
    internal class RefundOrderCommandHandler : IRequestHandler<RefundOrderCommand, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderTransactionService _orderTransactionService;

        public RefundOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderTransactionService orderTransactionService)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderTransactionService = orderTransactionService;
        }

        public async Task<Result<Order>> Handle(RefundOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail("No order found!");
            if (order.PaymentStatus != PaymentStatus.Refunding)
                return Result.Fail("This order is not currently refundable!");
            order.PaymentStatus = PaymentStatus.Refunded;
            await _orderRepository.Update(order);
            return order;
        }
    }
}

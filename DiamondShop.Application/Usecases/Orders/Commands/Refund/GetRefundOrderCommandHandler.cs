using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;
using Newtonsoft.Json.Linq;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Usecases.Orders.Commands.DeliverComplete;
using DiamondShop.Application.Services.Models;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;

namespace DiamondShop.Application.Usecases.Orders.Commands.Refund
{
    public record GetRefundOrderCommand(string orderId, string accountId) : IRequest<Result<PaymentLinkResponse>>;
    internal class GetRefundOrderCommandHandler : IRequestHandler<GetRefundOrderCommand, Result<PaymentLinkResponse>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IPaymentService _paymentService;

        public GetRefundOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, ITransactionRepository transactionRepository, IOrderTransactionService orderTransactionService, IPaymentService paymentService)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _transactionRepository = transactionRepository;
            _orderTransactionService = orderTransactionService;
            _paymentService = paymentService;
        }

        public async Task<Result<PaymentLinkResponse>> Handle(GetRefundOrderCommand request, CancellationToken token)
        {
            //TODO:Add get refund for customer
            request.Deconstruct(out string orderId, out string accountId);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail("This order doesn't exist");
            if (order.AccountId == AccountId.Parse(accountId))
                return Result.Fail("You don't have permission to get refund from this order");
            
            //TODO: Ignore this
            order.PaymentStatus = PaymentStatus.Refunded;
            await _orderRepository.Update(order);
            await _unitOfWork.BeginTransactionAsync(token);
            await _unitOfWork.CommitAsync(token);
            return null;
        }
    }
}

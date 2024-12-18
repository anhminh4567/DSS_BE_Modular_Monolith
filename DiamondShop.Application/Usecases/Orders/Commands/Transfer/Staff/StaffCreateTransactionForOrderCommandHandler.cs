using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ErrorMessages;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.Models.Transactions.ErrorMessages;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Commands.Transfer.Staff
{
    public record StaffCreateTransactionForOrderCommand(string verifierId, string orderId, string paymentMethodId) : IRequest<Result<Order>>;
    internal class StaffCreateTransactionForOrderCommandHandler : IRequestHandler<StaffCreateTransactionForOrderCommand, Result<Order>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IAccountRepository _accountRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public StaffCreateTransactionForOrderCommandHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ITransactionRepository transactionRepository, IOrderLogRepository orderLogRepository, IOrderService orderService, IOrderTransactionService orderTransactionService, IAccountRepository accountRepository, IPaymentMethodRepository paymentMethodRepository)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _transactionRepository = transactionRepository;
            _orderLogRepository = orderLogRepository;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
            _accountRepository = accountRepository;
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<Result<Order>> Handle(StaffCreateTransactionForOrderCommand request, CancellationToken token)
        {
            var parsedOrderId = OrderId.Parse(request.orderId);
            var parsedAccountId = AccountId.Parse(request.verifierId);
            var parsedMethodId = PaymentMethodId.Parse(request.paymentMethodId);
            var order = await _orderRepository.GetById(parsedOrderId);
            if(order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            var staff = await _accountRepository.GetById(parsedAccountId);
            if (staff == null)
                return Result.Fail(AccountErrors.AccountNotFoundError);
            var allPaymentMethod = await _paymentMethodRepository.GetAll();
            var chosenMethod = allPaymentMethod.FirstOrDefault(x => x.Id == parsedMethodId);
            if(chosenMethod == null)
                return Result.Fail(TransactionErrors.PaymentMethodErrors.NotFoundError);
            var payAmount = _orderTransactionService.GetCorrectAmountFromOrder(order);
            var manualPayment = Transaction.CreateManualPayment(order.Id, "ACB", "NGUYENVANA", $"Chuyển tiền còn lại từ tài khoản {order.Account?.FullName.FirstName} {order.Account?.FullName.LastName} cho đơn hàng {order.OrderCode}", payAmount, TransactionType.Pay);
            _transactionRepository.Create(manualPayment, token).Wait();
            await _unitOfWork.SaveChangesAsync();
           // await _unitOfWork.BeginTransactionAsync(token);
            var getPayment = await _transactionRepository.GetById(manualPayment.Id);
            if (getPayment == null)
                return Result.Fail(TransactionErrors.TransactionNotFoundError);
            return order;
        }
    }
}

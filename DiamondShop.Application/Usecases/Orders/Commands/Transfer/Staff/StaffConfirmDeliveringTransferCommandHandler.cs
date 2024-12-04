using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Transfers;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.ErrorMessages;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Orders.Commands.Transfer.Staff
{
    public record StaffConfirmDeliveringTransferCommand(string AccountId, TransferConfirmRequestDto TransferCompleteRequestDto) : IRequest<Result<Order>>;
    internal class StaffConfirmDeliveringTransferCommandHandler : IRequestHandler<StaffConfirmDeliveringTransferCommand, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderService _orderService;
        private readonly ITransferFileService _transferFileService;

        public StaffConfirmDeliveringTransferCommandHandler(IOrderRepository orderRepository, IOrderLogRepository orderLogRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, IOrderTransactionService orderTransactionService, IOrderService orderService, ITransferFileService transferFileService)
        {
            _orderRepository = orderRepository;
            _orderLogRepository = orderLogRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _orderTransactionService = orderTransactionService;
            _orderService = orderService;
            _transferFileService = transferFileService;
        }

        public async Task<Result<Order>> Handle(StaffConfirmDeliveringTransferCommand request, CancellationToken token)
        {
            request.Deconstruct(out string accountId, out TransferConfirmRequestDto transferCompleteRequestDto);
            transferCompleteRequestDto.Deconstruct(out string transactionId, out string orderId, out decimal amount, out string transactionCode);
            await _unitOfWork.BeginTransactionAsync(token);
            var manualPayment = await _transactionRepository.GetById(TransactionId.Parse(transactionId));
            if (manualPayment == null)
                return Result.Fail(TransactionErrors.TransactionNotFoundError);
            else if (manualPayment.TotalAmount != amount)
                return Result.Fail(TransactionErrors.TransactionNotValid);
            var order = manualPayment.Order;
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            else if (order.Status != OrderStatus.Delivering && order.PaymentMethodId != PaymentMethod.BANK_TRANSFER.Id)
                return Result.Fail(OrderErrors.UnproceedableError);
            else if (DateTime.UtcNow > order.ExpiredDate)
                return Result.Fail(OrderErrors.ExpiredTimeDueError);
            manualPayment.VerifySuccess(AccountId.Parse(accountId), transactionCode);
            await _transactionRepository.Update(manualPayment, token);
            order.PaymentStatus = PaymentStatus.Paid;
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}

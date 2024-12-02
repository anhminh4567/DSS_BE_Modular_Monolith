using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces.Transfers;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.ErrorMessages;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
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
using DiamondShop.Api.Controllers.Orders.Cancel;
using DiamondShop.Application.Usecases.Orders.Commands.Reject;

namespace DiamondShop.Application.Usecases.Orders.Commands.Transfer.Staff
{
    public record StaffRejectTransferCommand(string AccountId, TransferRejectRequestDto TransferRejectRequestDto) : IRequest<Result<Order>>;
    internal class StaffRejectTransferCommandHandler : IRequestHandler<StaffRejectTransferCommand, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderService _orderService;
        private readonly ITransferFileService _transferFileService;
        private readonly ISender _sender;

        public StaffRejectTransferCommandHandler(IOrderRepository orderRepository, IOrderLogRepository orderLogRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, IOrderTransactionService orderTransactionService, IOrderService orderService, ITransferFileService transferFileService, ISender sender)
        {
            _orderRepository = orderRepository;
            _orderLogRepository = orderLogRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _orderTransactionService = orderTransactionService;
            _orderService = orderService;
            _transferFileService = transferFileService;
            _sender = sender;
        }

        public async Task<Result<Order>> Handle(StaffRejectTransferCommand request, CancellationToken token)
        {
            request.Deconstruct(out string accountId, out TransferRejectRequestDto transferRejectRequestDto);
            transferRejectRequestDto.Deconstruct(out string transactionId, out string orderId);
            await _unitOfWork.BeginTransactionAsync(token);
            var manualPayment = await _transactionRepository.GetById(TransactionId.Parse(transactionId));
            if (manualPayment == null)
                return Result.Fail(TransactionErrors.TransactionNotFoundError);
            var order = manualPayment.Order;
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            else if (order.Status != OrderStatus.Pending && order.PaymentMethodId != PaymentMethod.BANK_TRANSFER.Id)
                return Result.Fail(OrderErrors.UnproceedableError);
            else if (DateTime.UtcNow > order.ExpiredDate)
                return Result.Fail(OrderErrors.ExpiredTimeDueError);
            manualPayment.VerifyFail(AccountId.Parse(accountId));
            await _transactionRepository.Update(manualPayment, token);
            await _unitOfWork.SaveChangesAsync(token);
            var rejectResult = await _sender.Send(new RejectOrderCommand(orderId, TransactionErrors.TransactionNotValid.Message));
            if (rejectResult.IsFailed)
                return Result.Fail(rejectResult.Errors);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}

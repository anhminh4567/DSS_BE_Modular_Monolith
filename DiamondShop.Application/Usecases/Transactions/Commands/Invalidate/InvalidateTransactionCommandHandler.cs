using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.ErrorMessages;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Transactions.Commands.Invalidate
{
    public record InvalidateTransactionCommand(string orderId, string transactionId, string invalidateReasons) : IRequest<Result<Transaction>>;
    internal class InvalidateTransactionCommandHandler : IRequestHandler<InvalidateTransactionCommand, Result<Transaction>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOrderRepository _orderRepository;

        public InvalidateTransactionCommandHandler(IUnitOfWork unitOfWork, ITransactionRepository transactionRepository, IOrderRepository orderRepository)
        {
            _unitOfWork = unitOfWork;
            _transactionRepository = transactionRepository;
            _orderRepository = orderRepository;
        }

        public async Task<Result<Transaction>> Handle(InvalidateTransactionCommand request, CancellationToken cancellationToken)
        {
            var parsedOrderId = OrderId.Parse(request.orderId);
            var parsedTransactionId = TransactionId.Parse(request.transactionId);
            
            var order = await _orderRepository.GetById(parsedOrderId);
            if(order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            
            var transaction = await _transactionRepository.GetById(parsedTransactionId);
            if(transaction == null)
                return Result.Fail(TransactionErrors.TransactionNotFoundError);
            if(transaction.OrderId != order.Id)
                return Result.Fail(TransactionErrors.TransactionNotFoundError);

            transaction.Invalidate(request.invalidateReasons);
            await _transactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
            return transaction;
        }
    }
    
}

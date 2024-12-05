using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
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

namespace DiamondShop.Application.Usecases.Transactions.Commands.Delete
{
    public record DeleteTransactionCommand(string orderId, string transactionId): IRequest<Result>;
    internal class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOrderRepository _orderRepository;

        public DeleteTransactionCommandHandler(IUnitOfWork unitOfWork, ITransactionRepository transactionRepository, IOrderRepository orderRepository)
        {
            _unitOfWork = unitOfWork;
            _transactionRepository = transactionRepository;
            _orderRepository = orderRepository;
        }

        public async Task<Result> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
        {
            var parsedOrderId = OrderId.Parse(request.orderId);
            var parsedTransactionId = TransactionId.Parse(request.transactionId);
            var order = await _orderRepository.GetById(parsedOrderId);  
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            var transaction  = order.Transactions.FirstOrDefault(x => x.Id == parsedTransactionId);
            if(transaction == null)
                return Result.Fail(TransactionErrors.TransactionNotFoundError);

            if(transaction.Status == Domain.Models.Transactions.Enum.TransactionStatus.Verifying)
                return Result.Fail(TransactionErrors.DeleteUnallowed);

            await _transactionRepository.Delete(transaction);
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
            throw new NotImplementedException();
        }
    }
}

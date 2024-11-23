using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
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

namespace DiamondShop.Application.Usecases.Transactions.Commands.AddManualRefunds
{
    public record AddManualRefundCommand(string orderId, string description,decimal fineAmount = 0) : IRequest<Result<Transaction>>;
    internal class AddManualRefundCommandHandler : IRequestHandler<AddManualRefundCommand, Result<Transaction>>
    {
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderRepository _orderRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddManualRefundCommandHandler( IOrderTransactionService orderTransactionService, IOrderRepository orderRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
        {
            _orderTransactionService = orderTransactionService;
            _orderRepository = orderRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Transaction>> Handle(AddManualRefundCommand request, CancellationToken cancellationToken)
        {
            var parsedId = OrderId.Parse(request.orderId);
            var getOrder = await _orderRepository.GetById(parsedId);  
            if(getOrder == null)
                return Result.Fail(new NotFoundError("Order not found"));
             decimal refundAmount =  _orderTransactionService.GetRefundAmountFromOrder(getOrder, request.fineAmount);
            Transaction newtrans = Transaction.CreateManualRefund(getOrder.Id,request.description,refundAmount - request.fineAmount, request.fineAmount);
            getOrder.AddRefund(newtrans);
            await _unitOfWork.BeginTransactionAsync();
            await _transactionRepository.Create(newtrans);
            await _orderRepository.Update(getOrder);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return newtrans;
        }
    }
}

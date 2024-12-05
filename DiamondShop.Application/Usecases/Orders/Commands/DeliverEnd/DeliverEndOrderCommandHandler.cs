using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Commands.DeliverEnd
{
    public record DeliverEndOrderCommand(string orderId) : IRequest<Result>;
    internal class DeliverEndOrderCommandHandler : IRequestHandler<DeliverEndOrderCommand, Result>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public DeliverEndOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderService orderService, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, ITransactionRepository transactionRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _optionsMonitor = optionsMonitor;
            _transactionRepository = transactionRepository;
        }

        public async Task<Result> Handle(DeliverEndOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if (order.Status != OrderStatus.Delivery_Failed)
                return Result.Fail(OrderErrors.UnproceedableError);
            order.DeliverEnd();
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
    }
}

using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Application.Dtos.Responses.Transactions;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Queries.GetTransactionDetail
{

    public record GetOrderTransactionDetailQuery(string orderId) : IRequest<Result<OrderTransactionDetailDto>>;
    internal class GetOrderTransactionDetailQueryHandler : IRequestHandler<GetOrderTransactionDetailQuery, Result<OrderTransactionDetailDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IOrderTransactionService _orderTransactionService;

        public GetOrderTransactionDetailQueryHandler(IOrderRepository orderRepository, ITransactionRepository transactionRepository, IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IOrderTransactionService orderTransactionService)
        {
            _orderRepository = orderRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _orderTransactionService = orderTransactionService;
        }

        public async Task<Result<OrderTransactionDetailDto>> Handle(GetOrderTransactionDetailQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetById(OrderId.Parse( request.orderId));    
            if(order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            var orderRule = _optionsMonitor.CurrentValue.OrderRule;
            var orderPaymentRule = _optionsMonitor.CurrentValue.OrderPaymentRules;
            var response = new OrderTransactionDetailDto();
            var getOrderTransaction = await _transactionRepository.GetByOrderId(order.Id);
            getOrderTransaction = getOrderTransaction.OrderByDescending(x => x.InitDate).ToList();
            response.Transactions = _mapper.Map<List<TransactionDto>>(getOrderTransaction);
            response.PaymentRules = orderPaymentRule;
            response.PaymentStatus = order.PaymentStatus;
            response.OrderStatus = order.Status;
            response.PaymentType = order.PaymentType;
            response.IsCancelled = (order.Status == Domain.Models.Orders.Enum.OrderStatus.Cancelled || order.Status == Domain.Models.Orders.Enum.OrderStatus.Rejected);
            if (response.IsCancelled)
            {
                response.IsShopFault = order.Status == Domain.Models.Orders.Enum.OrderStatus.Rejected;
            }
            response.DepositAmount = order.DepositFee;
            response.TotalAmount = order.TotalPrice;
            response.IsRefundable = order.PaymentStatus == Domain.Models.Orders.Enum.PaymentStatus.Refunding;
            response.ExpectedRefundAmount = _orderTransactionService.GetRefundAmountFromOrder(order,0,orderPaymentRule);
            return response;
        }

    }
}

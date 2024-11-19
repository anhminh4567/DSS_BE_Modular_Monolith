using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Api.Controllers.Orders.AssignDeliverer
{
    public record AssignDelivererOrderCommand(string orderId, string delivererId) : IRequest<Result<Order>>;
    internal class AssignDelivererOrderCommandHandler : IRequestHandler<AssignDelivererOrderCommand, Result<Order>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;

        public AssignDelivererOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IAccountRepository accountRepository, IOrderService orderService)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _orderService = orderService;
        }

        public async Task<Result<Order>> Handle(AssignDelivererOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string delivererId);
            await _unitOfWork.BeginTransactionAsync(token);
            var orderQuery = _orderRepository.GetQuery();
            var order = _orderRepository.QueryFilter(orderQuery, p => p.Id == OrderId.Parse(orderId)).FirstOrDefault();
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            await _orderService.AssignDeliverer(order, delivererId, _accountRepository, _orderRepository);
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
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

        public AssignDelivererOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IAccountRepository accountRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
        }

        public async Task<Result<Order>> Handle(AssignDelivererOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string delivererId);
            await _unitOfWork.BeginTransactionAsync(token);
            var account = await _accountRepository.GetById(AccountId.Parse(delivererId));
            if (account == null)
                return Result.Fail("This deliverer doesn't exist");
            if (account.Roles.Any(p => p.Id != AccountRole.Deliverer.Id))
                return Result.Fail("This account is not a deliverer");
            var orderQuery = _orderRepository.GetQuery();
            var order = _orderRepository.QueryFilter(orderQuery, p => p.Id == OrderId.Parse(orderId)).FirstOrDefault();
            if (order == null)
                return Result.Fail("The order doesn't exist");
            var conflictedOrderFlag = _orderRepository.QueryFilter(orderQuery, p => p.DelivererId == account.Id).Any(p => p.Status == OrderStatus.Prepared || p.Status == OrderStatus.Delivering);
            if (conflictedOrderFlag)
                return Result.Fail("This deliverer is currently unavailable");
            order.DelivererId = account.Id;
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}

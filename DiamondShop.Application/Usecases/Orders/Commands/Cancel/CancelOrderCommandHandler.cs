using DiamondShop.Application.Services.Data;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Api.Controllers.Orders.Cancel
{
    public record CancelOrderCommand(string orderId) : IRequest<Result<Order>>;
    public record CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Order>> Handle(CancelOrderCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync();
            var order = _orderRepository.GetQuery().FirstOrDefault();
            if (order == null) return Result.Fail(new ConflictError(""));
            order.Status = Domain.Models.Orders.Enum.OrderStatus.Cancelled;

            await _unitOfWork.CommitAsync();
            return order;
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.OrderItems.Command.Cancel
{
    public record CancelOrderItemCommand(List<string> orderItemId) : IRequest<Result<List<OrderItem>>>;
    internal class CancelOrderItemCommandHandler : IRequestHandler<CancelOrderItemCommand, Result<List<OrderItem>>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelOrderItemCommandHandler(IOrderItemRepository orderItemRepository, IUnitOfWork unitOfWork)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<OrderItem>>> Handle(CancelOrderItemCommand request, CancellationToken token)
        {
            request.Deconstruct(out List<string> orderItemId);
            await _unitOfWork.BeginTransactionAsync(token);
            var convertedIds = orderItemId.Select(p => OrderItemId.Parse(p));
            var itemQuery = _orderItemRepository.GetQuery();
            var items = _orderItemRepository.QueryFilter(itemQuery, p => convertedIds.Contains(p.Id) && p.Status != OrderItemStatus.Removed).ToList();
            items.ForEach(item => item.Status = OrderItemStatus.Removed);
            _orderItemRepository.UpdateRange(items);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return items;
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.OrderItems.Command.Prepare
{
    public record PrepareOrderItemCommand(List<string> orderItemIds) : IRequest<Result<List<OrderItem>>>;
    internal class PrepareOrderItemCommandHandler : IRequestHandler<PrepareOrderItemCommand, Result<List<OrderItem>>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PrepareOrderItemCommandHandler(IOrderItemRepository orderItemRepository, IUnitOfWork unitOfWork)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<OrderItem>>> Handle(PrepareOrderItemCommand request, CancellationToken token)
        {
            request.Deconstruct(out List<string> orderItemIds);
            await _unitOfWork.BeginTransactionAsync(token);
            var convertedIds = orderItemIds.Select(p => OrderItemId.Parse(p));
            var itemQuery = _orderItemRepository.GetQuery();
            var items = _orderItemRepository.QueryFilter(itemQuery, p => convertedIds.Contains(p.Id) && p.Status == OrderItemStatus.Preparing).ToList();
            if (items.Count == 0)
                return Result.Fail("No item found!");
            //check item of the same order
            var orderId = items[0].OrderId;
            foreach (var item in items)
            {
                if (orderId != item.OrderId)
                    return Result.Fail("Items need to be from the same order");
                item.Status = OrderItemStatus.Done;
            };
            _orderItemRepository.UpdateRange(items);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return items;
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Orders.Commands.PrepareItem
{
    public record PrepareOrderItemCommand(string ItemId) : IRequest<Result>;
    internal class PrepareOrderItemCommandHandler : IRequestHandler<PrepareOrderItemCommand, Result>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Task<Result> Handle(PrepareOrderItemCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

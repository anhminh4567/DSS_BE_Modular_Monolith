using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Deliveries.Commands.Begin
{
    public record BeginDeliveryCommand(string staffId) : IRequest<Result>;
    internal class BeginDeliveryCommandHandler : IRequestHandler<BeginDeliveryCommand, Result>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDeliveryPackageRepository _deliveryPackageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BeginDeliveryCommandHandler(IOrderRepository orderRepository, IDeliveryPackageRepository deliveryPackageRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _deliveryPackageRepository = deliveryPackageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(BeginDeliveryCommand request, CancellationToken token)
        {
            request.Deconstruct(out string staffId);
            await _unitOfWork.BeginTransactionAsync(token);

            var deliveryQuery = _deliveryPackageRepository.GetQuery();
            deliveryQuery = _deliveryPackageRepository.QueryFilter(deliveryQuery, p => p.DelivererId == AccountId.Parse(staffId));
            var delivery = deliveryQuery.FirstOrDefault();
            if (delivery == null)
                return Result.Fail("Can't find the delivering package for this staff");
            if (delivery.DelivererId != AccountId.Parse(staffId))
                return Result.Fail("Only the deliverer has the permission to complete this order");
            if (delivery.Status != DeliveryPackageStatus.Preparing)
                return Result.Fail("This package is currently not in preparing stage");
            delivery.Status = DeliveryPackageStatus.Delivering;
            delivery.DeliveryDate = DateTime.UtcNow.ToUniversalTime();
            await _deliveryPackageRepository.Update(delivery);
            await _unitOfWork.SaveChangesAsync(token);

            var orderQuery = _orderRepository.GetQuery();
            orderQuery = _orderRepository.QueryInclude(orderQuery, p => p.Items);
            orderQuery = _orderRepository.QueryFilter(orderQuery, p => p.DeliveryPackageId == delivery.Id);
            var orders = orderQuery.ToList();
            if (orders.Count == 0)
                return Result.Fail("Can't find any order in package");
            orders.ForEach(p => { if (p.Status == OrderStatus.Prepared) p.Status = OrderStatus.Delivering; });
            _orderRepository.UpdateRange(orders);
            await _unitOfWork.SaveChangesAsync(token);

            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
    }
}

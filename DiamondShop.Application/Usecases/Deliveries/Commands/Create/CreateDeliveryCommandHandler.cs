using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Deliveries.Commands.Create
{
    public record CreateDeliveryCommand(List<string> OrderIds, string DelivererId, DateTime DeliveryDate, string Method) : IRequest<Result>;
    internal class CreateDeliveryCommandHandler : IRequestHandler<CreateDeliveryCommand, Result>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IDeliveryPackageRepository _deliveryPackageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;

        public CreateDeliveryCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IDeliveryPackageRepository deliveryPackageRepository, IAccountRepository accountRepository, IOrderService orderService)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _deliveryPackageRepository = deliveryPackageRepository;
            _accountRepository = accountRepository;
            _orderService = orderService;
        }
        public async Task<Result> Handle(CreateDeliveryCommand request, CancellationToken token)
        {
            request.Deconstruct(out List<string> orderIds, out string delivererId, out DateTime deliveryDate, out string method);
            await _unitOfWork.BeginTransactionAsync(token);

            var staffQuery = _accountRepository.GetQuery();
            var staff = staffQuery.FirstOrDefault(p => p.Id == AccountId.Parse(delivererId));

            if (staff == null)
                return Result.Fail("This staff doesn't exist");

            var deliveryQuery = _deliveryPackageRepository.GetQuery();
            deliveryQuery = _deliveryPackageRepository.QueryFilter(deliveryQuery, p => p.DelivererId == staff.Id);
            deliveryQuery = _deliveryPackageRepository.QueryFilter(deliveryQuery, p => p.Status == DeliveryPackageStatus.Preparing || p.Status == DeliveryPackageStatus.Delivering);
            if (deliveryQuery.Any())
                return Result.Fail("This staff is currently on another delivering task");

            DeliveryPackage package = DeliveryPackage.Create(deliveryDate, method, staff.Id);
            await _deliveryPackageRepository.Create(package);

            var convertedIds = orderIds.Select(OrderId.Parse);
            var orderQuery = _orderRepository.GetQuery();
            orderQuery = _orderRepository.QueryInclude(orderQuery, p => p.Items);
            orderQuery = _orderRepository.QueryFilter(orderQuery, p => convertedIds.Contains(p.Id));
            var orders = orderQuery.ToList();
            if(orders.Count == 0)
                return Result.Fail("No order found.");
            //TODO: IMPLEMENT IT
            if (!_orderService.CheckForSameCity(orders))
                return Result.Fail("Orders need to arrive to the same city.");
            foreach (var order in orders)
            {
                if (order.Status != OrderStatus.Prepared)
                    return Result.Fail($"Order can only be delivered when it's {OrderStatus.Prepared.ToString().ToLower()}!");
                //TODO: Validate orders from the same address
                order.DeliveryPackageId = package.Id;
                order.Status = OrderStatus.Delivering;
            }
            _orderRepository.UpdateRange(orders);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            //TODO: Add notification
            return Result.Ok();
        }
    }
}

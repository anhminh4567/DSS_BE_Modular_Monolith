using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Domain.Models.Warranties.ErrorMessages;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;

namespace DiamondShop.Domain.Services.Implementations
{
    public class OrderService : IOrderService
    {
        List<OrderStatus> cancellableState = new() {
            OrderStatus.Pending,
            OrderStatus.Processing,
            OrderStatus.Prepared,
            OrderStatus.Delivery_Failed,
            OrderStatus.Delivering,
        };
        List<OrderStatus> ongoingState = new() {
            OrderStatus.Pending,
            OrderStatus.Processing,
            OrderStatus.Prepared,
            OrderStatus.Delivering,
        };
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IDiamondRepository _diamondRepository;

        public OrderService(IAccountRepository accountRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IJewelryRepository jewelryRepository, IDiamondRepository diamondRepository)
        {
            _accountRepository = accountRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _jewelryRepository = jewelryRepository;
            _diamondRepository = diamondRepository;
        }

        public bool CheckForSameCity(List<Order> orders)
        {
            //TODO: MAKE IT
            return true;
        }

        public bool IsCancellable(OrderStatus status)
        {
            return cancellableState.Contains(status);
        }
        public bool IsProceedable(OrderStatus status)
        {
            return ongoingState.Contains(status);
        }
        public async Task<Result> CancelItems(Order order)
        {
            var orderItemQuery = _orderItemRepository.GetQuery();
            orderItemQuery = _orderItemRepository.QueryFilter(orderItemQuery, p => p.OrderId == order.Id);
            var items = orderItemQuery.ToList();
            List<IError> errors = new();
            List<Jewelry> jewelries = new List<Jewelry>();
            List<Diamond> diamonds = new List<Diamond>();
            foreach (var item in items)
            {
                item.Status = OrderItemStatus.Removed;
                if (item.JewelryId != null)
                {
                    var jewelry = await _jewelryRepository.GetById(item.JewelryId);
                    if (jewelry.Status == ProductStatus.PreOrder)
                    {
                        await _jewelryRepository.Delete(jewelry);
                        if (jewelry.Diamonds != null)
                        {
                            foreach (var diamond in jewelry.Diamonds)
                            {
                                if (diamond.Status == ProductStatus.PreOrder)
                                {
                                    await _diamondRepository.Delete(diamond);
                                }
                                else
                                {
                                    diamond.SetSell();
                                    diamonds.Add(diamond);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (jewelry.Diamonds != null)
                        {
                            foreach (var diamond in jewelry.Diamonds)
                            {
                                diamond.SetForJewelry(jewelry);
                                diamonds.Add(diamond);
                            }
                        }
                    }
                    jewelry.SetSell();
                    jewelries.Add(jewelry);
                }
                if (item.DiamondId != null)
                {
                    var diamond = await _diamondRepository.GetById(item.DiamondId);
                    if (diamond == null)
                        errors.Append(new Error($"Can't find diamond #{item.DiamondId}"));
                    else
                    {
                        diamond.SetSell();
                        diamonds.Add(diamond);
                    }
                }
            }
            if (errors.Count > 0)
                return Result.Fail(errors);
            _orderItemRepository.UpdateRange(items);
            _jewelryRepository.UpdateRange(jewelries);
            _diamondRepository.UpdateRange(diamonds);
            return Result.Ok();
        }

        public Result CheckWarranty(string? jewelryId, string? diamondId, WarrantyType warrantyType)
        {
            if (jewelryId != null && diamondId == null)
            {
                if (warrantyType != WarrantyType.Jewelry)
                {
                    return Result.Fail(WarrantyErrors.WrongJewelryError);
                }
            }
            else if (diamondId != null)
            {
                if (warrantyType != WarrantyType.Diamond)
                {
                    return Result.Fail(WarrantyErrors.WrongDiamondError);
                }
            }
            return Result.Ok();
        }
        public async Task<Result<Order>> AssignDeliverer(Order order, string delivererId)
        {
            var account = await _accountRepository.GetById(AccountId.Parse(delivererId));
            if (account == null)
                return Result.Fail("This deliverer doesn't exist");
            if (account.Roles.Any(p => p.Id != AccountRole.Deliverer.Id))
                return Result.Fail("This account is not a deliverer");
            var orderQuery = _orderRepository.GetQuery();
            var conflictedOrderFlag = _orderRepository.QueryFilter(orderQuery, p => p.DelivererId == account.Id && p.Id != order.Id).Any(p => p.Status == OrderStatus.Prepared || p.Status == OrderStatus.Delivering);
            if (conflictedOrderFlag)
                return Result.Fail("This deliverer is currently unavailable");
            order.DelivererId = account.Id;
            return order;
        }
    }
}

using DiamondShop.Application.Dtos.Requests.Carts;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Orders.Commands.Create
{
    public record CreateOrderInfo(PaymentType PaymentType, string PaymentName, string? PromotionId, string Address, List<OrderItemRequestDto> OrderItemRequestDtos);
    public record CreateOrderCommand(string AccountId, CreateOrderInfo CreateOrderInfo, string? ParentOrderId = null) : IRequest<Result<Order>>;
    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Order>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISender _sender;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;

        public CreateOrderCommandHandler(IAccountRepository accountRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IDiamondRepository diamondRepository, IJewelryRepository jewelryRepository, IUnitOfWork unitOfWork, ISender sender, IOrderService orderService, IOrderTransactionService orderTransactionService)
        {
            _accountRepository = accountRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _diamondRepository = diamondRepository;
            _jewelryRepository = jewelryRepository;
            _unitOfWork = unitOfWork;
            _sender = sender;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
        }

        public async Task<Result<Order>> Handle(CreateOrderCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out string accountId, out CreateOrderInfo createOrderInfo, out string? parentOrderId);
            createOrderInfo.Deconstruct(out PaymentType paymentType, out string paymentName, out string? promotionId, out string address, out List<OrderItemRequestDto> orderItemReqs);
            var account = await _accountRepository.GetById(AccountId.Parse(accountId));
            if (account == null)
                return Result.Fail("This account doesn't exist");
            //TODO: Validate account status
            List<IError> errors = new List<IError>();
            var items = orderItemReqs.Select((item) =>
            {
                CartItemRequestDto cartItemRequest = new();
                var result = _orderService.CheckWarranty(item.JewelryId, item.DiamondId, item.WarrantyType);
                if (result.IsFailed)
                    errors.AddRange(result.Errors);
                cartItemRequest.Id = DateTime.UtcNow.Ticks.ToString();
                cartItemRequest.JewelryId = item.JewelryId;
                cartItemRequest.EngravedFont = item.EngravedFont;
                cartItemRequest.EngravedText = item.EngravedText;
                cartItemRequest.DiamondId = item.DiamondId;
                return cartItemRequest;
            }).ToList();
            if (errors.Count > 0)
                return Result.Fail(errors);
            else
                errors = new List<IError>();
            CartRequestDto cartRequestDto = new CartRequestDto()
            {
                PromotionId = promotionId,
                Items = items
            };
            //Validate CartModel
            var cartModelResult = await _sender.Send(new ValidateCartFromListCommand(cartRequestDto));
            if (cartModelResult.IsFailed)
                return Result.Fail(cartModelResult.Errors);

            var cartModel = cartModelResult.Value;
            if (!cartModel.OrderValidation.IsOrderValid)
            {
                foreach (var index in cartModel.OrderValidation.UnavailableItemIndex)
                {
                    errors.Add(new Error(cartModel.Products[index].ErrorMessage));
                }
                foreach (var index in cartModel.OrderValidation.UnavailableItemIndex)
                {
                    errors.Add(new Error(cartModel.Products[index].ErrorMessage));
                }
            }
            if (errors.Count > 0) return Result.Fail(errors);
            //TODO: ADD seperate error validation
            //    foreach (var index in cartModel.OrderValidation.InvalidItemIndex)
            //    {
            //        var invalidItem = cartModel.Products[index];
            //        var error = Result.Fail(cartModel);
            //    }

            var orderPromo = cartModel.Promotion.Promotion;
            var order = Order.Create(account.Id, paymentType, cartModel.OrderPrices.FinalPrice, cartModel.ShippingPrice.FinalPrice,
                address, orderPromo?.Id);
            //if replacement order
            if (parentOrderId != null)
                order.ParentOrderId = OrderId.Parse(parentOrderId);
            await _orderRepository.Create(order, token);
            List<OrderItem> orderItems = new();
            List<Jewelry> jewelries = new();
            List<Diamond> diamonds = new();
            foreach (var product in cartModel.Products)
            {
                string giftedId = product.Diamond?.Id?.Value ?? product.Jewelry?.Id?.Value;
                var gift = giftedId is null ? null : orderPromo?.Gifts.FirstOrDefault(k => k.ItemId == giftedId);
                //If shop replacement, then bought price should be 0
                //TODO: Add final price
                orderItems.Add(OrderItem.Create(order.Id, product.Jewelry?.Id, product.Diamond?.Id,
                    0, product.ReviewPrice.FinalPrice,
                product.DiscountId, product.DiscountPercent,
                gift?.UnitType, gift?.UnitValue));
                if (product.Jewelry != null)
                {
                    product.Jewelry.SetSold();
                    jewelries.Add(product.Jewelry);
                }
                if (product.Diamond != null)
                {
                    product.Diamond.SetSold(product.ReviewPrice.DefaultPrice, product.ReviewPrice.FinalPrice);
                    diamonds.Add(product.Diamond);
                }
            }
            await _orderItemRepository.CreateRange(orderItems);
            _jewelryRepository.UpdateRange(jewelries);
            _diamondRepository.UpdateRange(diamonds);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            order.Account = account;
            return order;
        }

    }
}

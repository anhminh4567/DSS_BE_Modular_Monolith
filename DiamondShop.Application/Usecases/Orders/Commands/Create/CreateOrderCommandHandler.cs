using DiamondShop.Application.Dtos.Requests.Carts;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace DiamondShop.Application.Usecases.Orders.Commands.Create
{
    public record CreateOrderInfo(PaymentType PaymentType, string methodId , string PaymentName, string? RequestId, string? PromotionId, string Address, List<OrderItemRequestDto> OrderItemRequestDtos);
    public record CreateOrderCommand(string AccountId, CreateOrderInfo CreateOrderInfo, Order? ParentOrder = null, List<OrderItem> ParentItems = null) : IRequest<Result<Order>>;
    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Order>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISender _sender;
        private readonly IOrderService _orderService;
        private readonly IJewelryService _jewelryService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IEmailService _emailService;

        public CreateOrderCommandHandler(IAccountRepository accountRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IDiamondRepository diamondRepository, ISizeMetalRepository sizeMetalRepository, IJewelryRepository jewelryRepository, IUnitOfWork unitOfWork, ISender sender, IOrderService orderService, IJewelryService jewelryService, IOrderTransactionService orderTransactionService, IPaymentMethodRepository paymentMethodRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IOrderLogRepository orderLogRepository, IEmailService emailService)
        {
            _accountRepository = accountRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _diamondRepository = diamondRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _jewelryRepository = jewelryRepository;
            _unitOfWork = unitOfWork;
            _sender = sender;
            _orderService = orderService;
            _jewelryService = jewelryService;
            _orderTransactionService = orderTransactionService;
            _paymentMethodRepository = paymentMethodRepository;
            _optionsMonitor = optionsMonitor;
            _orderLogRepository = orderLogRepository;
            _emailService = emailService;
        }

        public async Task<Result<Order>> Handle(CreateOrderCommand request, CancellationToken token)
        {
            var transactionRule = _optionsMonitor.CurrentValue.TransactionRule;
            var logRule = _optionsMonitor.CurrentValue.LoggingRules;
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out string accountId, out CreateOrderInfo createOrderInfo, out Order? parentOrder, out List<OrderItem> parentItems);
            createOrderInfo.Deconstruct(out PaymentType paymentType, out string methodId, out string paymentName, out string? requestId, out string? promotionId, out string address, out List<OrderItemRequestDto> orderItemReqs);
            var account = await _accountRepository.GetById(AccountId.Parse(accountId));
            if (account == null)
                return Result.Fail("This account doesn't exist");
            PaymentMethodId parrsedMethodId = PaymentMethodId.Parse(methodId);
            var getAllPaymentMethod = await _paymentMethodRepository.GetAll();
            var paymentMethod = getAllPaymentMethod.FirstOrDefault(p => p.Id == parrsedMethodId);
            if(paymentMethod == null)
                return Result.Fail("This payment method doesn't exist");
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
                //??
                cartItemRequest.WarrantyCode = item.WarrantyCode;
                cartItemRequest.WarrantyType = item.WarrantyType;
                return cartItemRequest;
            }).ToList();
            if (errors.Count > 0)
                return Result.Fail(errors);
            else
                errors = new List<IError>();
            CartRequestDto cartRequestDto = new CartRequestDto()
            {
                PromotionId = promotionId,
                Items = items,
                AccountId = account.Id.Value
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
            }
            if (errors.Count > 0) return Result.Fail(errors);
            //TODO: ADD seperate error validation
            //    foreach (var index in cartModel.OrderValidation.InvalidItemIndex)
            //    {
            //        var invalidItem = cartModel.Products[index];
            //        var error = Result.Fail(cartModel);
            //    }
            if(paymentMethod.Id == PaymentMethod.ZALOPAY.Id)
            {
                if (cartModelResult.Value.OrderPrices.FinalPrice > transactionRule.MaximumPerTransaction)
                    return Result.Fail("Transaction value is too high for method "+ PaymentMethod.ZALOPAY.MethodName);
            }

            var customizeRequestId = requestId == null ? null : CustomizeRequestId.Parse(requestId);
            var orderPromo = cartModel.Promotion.Promotion;
            var order = Order.Create(account.Id, paymentType, paymentMethod.Id, cartModel.OrderPrices.FinalPrice, cartModel.ShippingPrice.FinalPrice,
                address, customizeRequestId, orderPromo?.Id, cartModel.OrderPrices.OrderAmountSaved, cartModel.OrderPrices.UserRankDiscountAmount);
            //create log
            var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Pending);
            //if replacement order
            if (parentOrder != null)
            {
                order.ParentOrderId = parentOrder.Id;
                order.TotalPrice += parentItems.Sum(p => p.PurchasedPrice);
                order.Status = OrderStatus.Processing;
            }
            await _orderRepository.Create(order, token);
            await _orderLogRepository.Create(log, token);
            if(order.ParentOrderId != null)
            {
                var continueLog = OrderLog.CreateByChangeStatus(order, OrderStatus.Processing);
                await _orderLogRepository.Create(continueLog, token);
            }
            List<OrderItem> orderItems = new();
            List<Jewelry> jewelries = new();
            List<Diamond> diamonds = new();
            foreach (var product in cartModel.Products)
            {
                string giftedId = product.Diamond?.Id?.Value ?? product.Jewelry?.Id?.Value;
                var gift = giftedId is null ? null : orderPromo?.Gifts.FirstOrDefault(k => k.ItemId == giftedId);
                //If shop replacement, then bought price should be 0
                //TODO: Add final price
                orderItems.Add(OrderItem.Create(order.Id, product.Jewelry?.Id, product.Diamond?.Id,product.ReviewPrice.DefaultPrice,
                     product.ReviewPrice.FinalPrice,
                product.DiscountId, product.DiscountPercent,
                gift?.UnitType, gift?.UnitValue,product.CurrentWarrantyPrice));
                if (product.Jewelry != null)
                {
                    _jewelryService.AddPrice(product.Jewelry, _sizeMetalRepository);
                    product.Jewelry.SetSold(product.Jewelry.ND_Price.Value, product.ReviewPrice.DefaultPrice,product.EngravedText, product.EngravedFont);
                    jewelries.Add(product.Jewelry);
                }
                if (product.Diamond != null)
                {
                    product.Diamond.SetSold(product.ReviewPrice.DefaultPrice, product.ReviewPrice.FinalPrice);
                    diamonds.Add(product.Diamond);
                }
            }
            if (parentItems != null)
            {
                foreach (var item in parentItems)
                {
                    orderItems.Add(OrderItem.Create(order.Id, item.JewelryId, item.DiamondId,  item.PurchasedPrice,item.OriginalPrice,
                        item.DiscountId, item.DiscountPercent, item.PromoType, item.PromoValue));
                }
            }
            await _orderItemRepository.CreateRange(orderItems);
            _jewelryRepository.UpdateRange(jewelries);
            _diamondRepository.UpdateRange(diamonds);


            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            order.Account = account;
            //no wait to send email
            _emailService.SendInvoiceEmail(order,account);
            return order;
        }

    }
}

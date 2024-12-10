using DiamondShop.Application.Dtos.Requests.Carts;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson;
using DiamondShop.Application.Usecases.Orders.Commands.Proceed;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ErrorMessages;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Notifications;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.ErrorMessages;
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

namespace DiamondShop.Application.Usecases.Orders.Commands.Create
{
    public record CreateOrderInfo(PaymentType PaymentType, string methodId, string PaymentName, string? RequestId, string? PromotionId, BillingDetail BillingDetail, List<OrderItemRequestDto> OrderItemRequestDtos,bool? IsAtShopOrder = false);
    public record CreateOrderCommand(string AccountId, CreateOrderInfo CreateOrderInfo) : IRequest<Result<Order>>;
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
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IEmailService _emailService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IOptions<LocationRules> _locationOptions;
        private readonly IOrderTransactionService _orderTransactionService;

        public CreateOrderCommandHandler(IAccountRepository accountRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IDiamondRepository diamondRepository, ISizeMetalRepository sizeMetalRepository, IJewelryRepository jewelryRepository, IUnitOfWork unitOfWork, ISender sender, IOrderService orderService, IJewelryService jewelryService, IPaymentMethodRepository paymentMethodRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IOrderLogRepository orderLogRepository, IEmailService emailService, INotificationRepository notificationRepository, IOptions<LocationRules> locationOptions, IOrderTransactionService orderTransactionService)
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
            _paymentMethodRepository = paymentMethodRepository;
            _optionsMonitor = optionsMonitor;
            _orderLogRepository = orderLogRepository;
            _emailService = emailService;
            _notificationRepository = notificationRepository;
            _locationOptions = locationOptions;
            _orderTransactionService = orderTransactionService;
        }

        public async Task<Result<Order>> Handle(CreateOrderCommand request, CancellationToken token)
        {
            var transactionRule = _optionsMonitor.CurrentValue.TransactionRule;
            var logRule = _optionsMonitor.CurrentValue.LoggingRules;
            var paymentRule = _optionsMonitor.CurrentValue.OrderPaymentRules;
            var orderRule = _optionsMonitor.CurrentValue.OrderRule;
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out string accountId, out CreateOrderInfo createOrderInfo);
            createOrderInfo.Deconstruct(out PaymentType paymentType, out string methodId, out string paymentName, out string? requestId, out string? promotionId, out BillingDetail billingDetail, out List<OrderItemRequestDto> orderItemReqs, out bool? isAtShop);
            var account = await _accountRepository.GetById(AccountId.Parse(accountId));
            if (account == null)
                return Result.Fail(AccountErrors.AccountNotFoundError);
            PaymentMethodId parrsedMethodId = PaymentMethodId.Parse(methodId);
            var getAllPaymentMethod = await _paymentMethodRepository.GetAll();
            var paymentMethod = getAllPaymentMethod.FirstOrDefault(p => p.Id == parrsedMethodId);
            if (paymentMethod == null)
                return Result.Fail(TransactionErrors.PaygateError.PaygateNotFoundError);
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
                AccountId = account.Id.Value,
                UserAddress = new Dtos.Requests.Accounts.AddressRequestDto()
                {
                    District = billingDetail.District,
                    Province = billingDetail.Providence,
                    Street = billingDetail.Address,
                    Ward = billingDetail.Ward
                },
                IsAtShopOrder = isAtShop.Value
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
                foreach (var errMess in cartModel.OrderValidation.MainErrorMessage)
                {
                    errors.Add(new Error(errMess));
                }
            }
            //check the pay method
            if (paymentType == PaymentType.Payall)
            {
                if (cartModel.OrderPrices.FinalPrice > orderRule.MaxOrderAmountForFullPayment)
                {
                    bool isCustomRequest = requestId != null;
                    var depositPercent = isCustomRequest ? paymentRule.DepositPercent : paymentRule.CODPercent;
                    errors.Add(new Error($"Tổng giá trị đơn hàng vượt quá giới hạn cho phép {orderRule.MaxOrderAmountForFullPayment}, xin vui lòng đặt cọc {depositPercent}%"));
                }
            }
            else
            {
                if (cartModel.OrderPrices.IsFreeOrder)
                    errors.Add(new Error("Đơn hàng miễn phí không được chọn loại COD"));
                if (cartModel.OrderPrices.FinalPrice > orderRule.MaxOrderAmountForDelivery)
                {
                    //errors.Add(new Error($"Tổng giá trị đơn hàng vượt quá giới hạn cho phép {orderRule.MaxCOD}"));
                }
            }
            //if(isAtShop.Value == false)
            //{
            //    if(cartModel.OrderPrices.FinalPrice > orderRule.MaxOrderAmountForDelivery)
            //        errors.Add(new Error($"Tổng giá trị đơn hàng vượt quá giới hạn cho phép {orderRule.MaxOrderAmountForDelivery}.VND để giao cho khách, vui lòng chọn nhận tại shop, xin lỗi vì sự bất tiện"));
            //}
            //else
            //{
            //    //if()
            //} 
            
                        
            if (errors.Count > 0)
                return Result.Fail(errors);
            var customizeRequestId = requestId == null ? null : CustomizeRequestId.Parse(requestId);
            var orderPromo = cartModel.Promotion.Promotion;
            string address = null;
            if(isAtShop != null && isAtShop.Value == true)
                address = "Tại cửa hàng, địa chỉ: "+ _locationOptions.Value.OriginalLocationName;
            else
                address = billingDetail.GetAddressString();
            decimal depositFee = paymentType == PaymentType.Payall ? 0m : (0.01m * paymentRule.CODPercent) * cartModel.OrderPrices.FinalPrice;
            depositFee = MoneyVndRoundUpRules.RoundAmountFromDecimal(depositFee);
            DateTime? expiredDate = DateTime.UtcNow.AddHours(orderRule.ExpiredOrderHour);//paymentMethod.Id == PaymentMethod.BANK_TRANSFER.Id ? DateTime.UtcNow.AddHours(paymentRule.CODHourTimeLimit) : null;
            var order = Order.Create(account.Id, paymentType, paymentMethod.Id, cartModel.OrderPrices.FinalPrice, cartModel.ShippingPrice.FinalPrice, depositFee,
                address, customizeRequestId, orderPromo, cartModel.OrderPrices.OrderAmountSaved, cartModel.OrderPrices.UserRankDiscountAmount,expiredDate);
            //create log
            if(isAtShop != null && isAtShop.Value == true)
            {
                order.ChangeToCollectAtShop();
            }

            //check if zalopay, check maximum 
            if (paymentMethod.Id == PaymentMethod.ZALOPAY.Id)
            {
                var expectedValue = _orderTransactionService.GetCorrectAmountFromOrder(order);
                if(expectedValue > paymentMethod.MaxSupportedPrice)
                {
                    errors.Add(TransactionErrors.PaygateError.MaxTransactionError(paymentName, (long)paymentMethod.MaxSupportedPrice ));
                }
                if (cartModelResult.Value.OrderPrices.FinalPrice > transactionRule.MaximumPerTransaction)
                   errors.Add(TransactionErrors.PaygateError.MaxTransactionError(paymentName, transactionRule.MaximumPerTransaction));
                //return Result.Fail( TransactionErrors.PaygateError.MaxTransactionError(paymentName,transactionRule.MaximumPerTransaction));
            }
            if (errors.Count > 0)
                return Result.Fail(errors);

            var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Pending);
            List<OrderItem> orderItems = new();
            List<Jewelry> jewelries = new();
            List<Diamond> diamonds = new();
            foreach (var product in cartModel.Products)
            {
                string giftedId = product.Diamond?.Id?.Value ?? product.Jewelry?.Id?.Value;
                var gift = giftedId is null ? null : orderPromo?.Gifts.FirstOrDefault(k => k.ItemCode == giftedId);
                //If shop replacement, then bought price should be 0
                //TODO: Add final price
                var getDiscountIfExist = cartModel.DiscountsApplied.FirstOrDefault(k => k.Id == product.DiscountId);
                orderItems.Add(OrderItem.Create(order.Id, product.Jewelry?.Id, product.Diamond?.Id, product.ReviewPrice.DefaultPrice,
                     product.ReviewPrice.FinalPrice,
                getDiscountIfExist, product.ReviewPrice.PromotionAmountSaved,
                product.ReviewPrice.DiscountAmountSaved, product.CurrentWarrantyPrice));
                if (requestId != null)
                {
                    if (product.Jewelry != null)
                    {
                        _jewelryService.AddPrice(product.Jewelry, _sizeMetalRepository);
                        product.Jewelry.SetSoldUnavailable( product.ReviewPrice.FinalPrice, product.EngravedText, product.EngravedFont);
                        jewelries.Add(product.Jewelry);
                    }
                }
                else
                {
                    if (product.Jewelry != null)
                    {
                        _jewelryService.AddPrice(product.Jewelry, _sizeMetalRepository);
                        product.Jewelry.SetSold(product.ReviewPrice.FinalPrice, product.EngravedText, product.EngravedFont);
                        jewelries.Add(product.Jewelry);
                    }
                    if (product.Diamond != null)
                    {
                        product.Diamond.SetSold(product.ReviewPrice.DefaultPrice, product.ReviewPrice.FinalPrice);
                        diamonds.Add(product.Diamond);
                    }
                }
            }
            var totalProductPromoSavedAmount = orderItems.Sum(k => k.PromotionSavedAmount);
            var totalOrderPromoSavedAmount = order.OrderAmountSaved + totalProductPromoSavedAmount;
            order.TotalPromotionAmountSaved = totalOrderPromoSavedAmount.Value;
            order.ShippingFeeSaved = cartModel.ShippingPrice.UserRankReducedPrice;
            await _orderRepository.Create(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _orderLogRepository.Create(log);
            await _orderItemRepository.CreateRange(orderItems);
            _jewelryRepository.UpdateRange(jewelries);
            _diamondRepository.UpdateRange(diamonds);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            order.Account = account;
            //no wait to send email
            _emailService.SendInvoiceEmail(order, account);
            var notificationToCustomer = Notification.CreateAccountMessage(order, account,"đơn hàng đã dược đặt, vui lòng thanh toán trong thời hạn",null);
            _notificationRepository.Create(notificationToCustomer).Wait();
            await _unitOfWork.SaveChangesAsync(token);
            // if order price = 0 then auto proceed;
            if (order.TotalPrice == 0)
            {
                var proceedOrderCommand = new ProceedOrderCommand(order.Id.Value, order.AccountId.Value);
                var result = await _sender.Send(proceedOrderCommand);
            }
            return order;
        }

    }
}

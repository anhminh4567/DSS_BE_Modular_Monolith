using DiamondShop.Application.Dtos.Requests.Carts;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Warranties.Enum;
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
    public record BillingDetail(string FirstName, string LastName, string Phone, string Email, string Providence, string District, string Ward, string Address, string? Note);
    public record CreateOrderInfo(OrderRequestDto OrderRequestDto, List<OrderItemRequestDto> OrderItemRequestDtos);
    public record CreateOrderCommand(string AccountId, BillingDetail BillingDetail, CreateOrderInfo CreateOrderInfo) : IRequest<Result<PaymentLinkResponse>>;
    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<PaymentLinkResponse>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMainDiamondService _mainDiamondService;
        private readonly IPaymentService _paymentService;
        private readonly ICartService _cartService;
        private readonly ICartModelService _cartModelService;
        private readonly ISender _sender;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IAccountRepository accountRepository, IUnitOfWork unitOfWork, IPaymentService paymentService, IJewelryRepository jewelryRepository, IMainDiamondRepository mainDiamondRepository, ISender sender, IDiamondRepository diamondRepository, IMainDiamondService mainDiamondService, IPaymentMethodRepository paymentMethodRepository, ICartService cartService)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _jewelryRepository = jewelryRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _sender = sender;
            _diamondRepository = diamondRepository;
            _mainDiamondService = mainDiamondService;
            _paymentMethodRepository = paymentMethodRepository;
            _cartService = cartService;
        }

        public async Task<Result<PaymentLinkResponse>> Handle(CreateOrderCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out string accountId, out BillingDetail billingDetail, out CreateOrderInfo createOrderInfo);
            createOrderInfo.Deconstruct(out OrderRequestDto orderReq, out List<OrderItemRequestDto> orderItemReqs);

            var account = await _accountRepository.GetById(AccountId.Parse(accountId));
            if (account == null)
                return Result.Fail("This account doesn't exist");
            //TODO: Validate account status

            List<IError> errors = new List<IError>();

            var items = orderItemReqs.Select((item) =>
            {
                CartItemRequestDto cartItemRequest = new();
                if (item.JewelryId != null && item.DiamondId == null)
                {
                    if (item.WarrantyType != WarrantyType.Jewelry)
                    {
                        errors.Add(new Error($"Wrong Type of warranty for jewelry #{item.JewelryId}"));
                    }
                }
                else if (item.DiamondId != null)
                {
                    if (item.WarrantyType != WarrantyType.Diamond)
                    {
                        errors.Add(new Error($"Wrong Type of warranty for diamond #{item.DiamondId}"));
                    }
                }
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
                PromotionId = orderReq.PromotionId,
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

            var order = Order.Create(account.Id, orderReq.PaymentType, cartModel.OrderPrices.FinalPrice, cartModel.ShippingPrice.FinalPrice, 
                String.Join(" ", [billingDetail.Providence, billingDetail.District, billingDetail.Ward, billingDetail.Address]), orderPromo?.Id);
            await _orderRepository.Create(order, token);
            List<OrderItem> orderItems = new();
            HashSet<Jewelry> jewelrySet = new();
            HashSet<Diamond> diamondSet = new();
            foreach (var product in cartModel.Products)
            {
                string giftedId = product.Diamond?.Id?.Value ?? product.Jewelry?.Id?.Value;
                var gift = giftedId is null ? null : orderPromo?.Gifts.FirstOrDefault(k => k.ItemId == giftedId);
                orderItems.Add(OrderItem.Create(order.Id, product.Jewelry?.Id, product.Diamond?.Id,
                product.EngravedText, product.EngravedFont, product.ReviewPrice.FinalPrice,
                product.DiscountId, product.DiscountPercent,
                gift?.UnitType, gift?.UnitValue));
                if (product.Jewelry != null) jewelrySet.Add(product.Jewelry);
                if (product.Diamond != null) diamondSet.Add(product.Diamond);
            }
            await _orderItemRepository.CreateRange(orderItems);

            var jewelries = jewelrySet.ToList();
            jewelries.ForEach(p => p.SetSold());
            _jewelryRepository.UpdateRange(jewelries);

            var diamonds = diamondSet.ToList();
            diamonds.ForEach(p => p.SetSold());
            _diamondRepository.UpdateRange(diamonds);

            await _unitOfWork.SaveChangesAsync(token);

            //Create Paymentlink if not transfer
            PaymentLinkRequest paymentLinkRequest = new PaymentLinkRequest()
            {
                Account = account,
                Order = order,
                Email = account.Email,
                Phone = billingDetail.Phone,
                Address = order.ShippingAddress,
                Title = "Payment",
                Description = $"Payment for order #{order.Id}",
                Amount = order.TotalPrice,
            };
            var paymentLink = await _paymentService.CreatePaymentLink(paymentLinkRequest, token);
            await _unitOfWork.CommitAsync(token);
            return paymentLink;

        }

    }
}

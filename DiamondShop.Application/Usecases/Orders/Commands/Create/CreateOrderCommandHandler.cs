using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Application.Usecases.Orders.Commands.Validate;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
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
using System.Collections.Generic;

namespace DiamondShop.Application.Usecases.Orders.Commands.Create
{
    public record BillingDetail(string FirstName, string LastName, string Phone, string Email, string Providence, string Ward, string Address, string Note);
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
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMainDiamondService _mainDiamondService;
        private readonly IPaymentService _paymentService;
        private readonly ICartModelService _cartModelService;
        private readonly ISender _sender;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IAccountRepository accountRepository, IUnitOfWork unitOfWork, IPaymentService paymentService, IJewelryRepository jewelryRepository, IMainDiamondRepository mainDiamondRepository, ISender sender, IDiamondRepository diamondRepository, IMainDiamondService mainDiamondService, IPaymentMethodRepository paymentMethodRepository)
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

            List<CartProduct> products = new();
            HashSet<Jewelry> jewelrySet = new();
            HashSet<Diamond> diamondSet = new();
            List<IError> errors = new List<IError>();
            foreach (var item in orderItemReqs)
            {
                if (item.JewelryId != null)
                {



                }
                CartProduct cartProduct = new();
                if (item.JewelryId != null)
                {
                    if (item.WarrantyType != WarrantyType.Jewelry)
                    {
                        errors.Add(new Error($"Wrong Type of warranty for jewelry #{item.JewelryId}"));
                        continue;
                    }
                    cartProduct.Jewelry = await _jewelryRepository.GetById(JewelryId.Parse(item.JewelryId));
                    cartProduct.EngravedFont = item.EngravedFont;
                    cartProduct.EngravedText = item.EngravedText;
                    jewelrySet.Add(cartProduct.Jewelry);
                }
                if (item.DiamondId != null)
                {
                    if (item.WarrantyType != WarrantyType.Diamond)
                    {
                        errors.Add(new Error($"Wrong Type of warranty for diamond #{item.DiamondId}"));
                        continue;
                    }
                    cartProduct.Diamond = await _diamondRepository.GetById(DiamondId.Parse(item.DiamondId));
                    if (item.JewelryId != null)
                    {
                        if (cartProduct.Diamond.JewelryId != null && cartProduct.Diamond.JewelryId != cartProduct.Jewelry?.Id)
                            errors.Add(new Error($"Diamond #{cartProduct.Diamond.Id} is already attached to another Jewelry"));
                        cartProduct.Diamond.JewelryId = JewelryId.Parse(item.JewelryId);
                    }
                    diamondSet.Add(cartProduct.Diamond);
                }
                products.Add(cartProduct);
            }
            if (errors.Count > 0)
                return Result.Fail(errors);
            else
                errors = new List<IError>();
            //Validate matching diamond
            foreach (var jewelry in jewelrySet)
            {
                var attachedDiamond = diamondSet.Where(p => p.JewelryId == jewelry.Id).ToList();
                var result = await _mainDiamondService.CheckMatchingDiamond(jewelry.ModelId, attachedDiamond, _mainDiamondRepository);
                if (result.IsFailed) errors.AddRange(result.Errors);
            }
            if (errors.Count > 0)
                return Result.Fail(errors);

            //Validate CartModel
            var cartModelResult = await _sender.Send(new ValidateOrderCommand(products));
            if (cartModelResult.IsFailed)
                return Result.Fail(cartModelResult.Errors);

            var jewelries = jewelrySet.ToList();
            jewelries.ForEach(p => p.SetSold());
            _jewelryRepository.UpdateRange(jewelries);

            var diamonds = diamondSet.ToList();
            diamonds.ForEach(p => p.SetSold());
            _diamondRepository.UpdateRange(diamonds);

            var cartModel = cartModelResult.Value;
            var orderPromo = cartModel.Promotion.Promotion;

            var order = Order.Create(account.Id, orderReq.PaymentType, cartModel.OrderPrices.FinalPrice, cartModel.ShippingPrice.FinalPrice, billingDetail.Address, orderPromo?.Id);
            await _orderRepository.Create(order, token);
            await _unitOfWork.SaveChangesAsync(token);

            List<OrderItem> orderItems = cartModel.Products.Select(p =>
            {
                string giftedId = p.Diamond?.Id?.Value ?? p.Jewelry?.Id?.Value;
                var gift = giftedId is null ? null : orderPromo?.Gifts.FirstOrDefault(k => k.ItemId == giftedId);
                return OrderItem.Create(order.Id, p.Jewelry?.Id, p.Diamond?.Id,
                p.EngravedText, p.EngravedFont, p.ReviewPrice.FinalPrice,
                p.DiscountId, p.DiscountPercent,
                gift?.UnitType, gift?.UnitValue);
            }).ToList();
            await _orderItemRepository.CreateRange(orderItems);
            await _unitOfWork.SaveChangesAsync(token);

            await _unitOfWork.CommitAsync(token);

            //Create Paymentlink if not transfer
            if (orderReq.IsTransfer) return new PaymentLinkResponse() { };
            string title = "";
            string callbackURL = "";
            string returnURL = "";
            string description = "";
            PaymentLinkRequest paymentLinkRequest = new PaymentLinkRequest()
            {
                Account = account,
                Order = order,
                Email = account.Email,
                Phone = billingDetail.Phone,
                Address = billingDetail.Address,
                Title = title,
                Description = description,
                Amount = order.TotalPrice,
            };
            return await _paymentService.CreatePaymentLink(paymentLinkRequest, token);
        }

    }
}

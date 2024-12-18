using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Domain.Models.AccountAggregate.ErrorMessages;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.Events;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DiamondShop.Application.Usecases.Orders.Commands.Proceed
{
    public record DelivererCompleteOrderRequestDto(IFormFile[]? confirmImages, IFormFile? confirmVideo);
    public record ProceedOrderCommand(string orderId, string? accountId, DelivererCompleteOrderRequestDto? CompleteOrderRequestDto = null) : IRequest<Result<Order>>;
    internal class ProceedOrderCommandHandler : IRequestHandler<ProceedOrderCommand, Result<Order>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly ISender _sender;
        private readonly IPublisher _publisher;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IOrderFileServices _orderFileServices;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IEmailService _emailService;

        public ProceedOrderCommandHandler(IAccountRepository accountRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, ITransactionRepository transactionRepository, IJewelryRepository jewelryRepository, IUnitOfWork unitOfWork, IOrderService orderService, IOrderTransactionService orderTransactionService, ISender sender, IPublisher publisher, IPaymentMethodRepository paymentMethodRepository, IOrderLogRepository orderLogRepository, IOrderFileServices orderFileServices, IDiamondRepository diamondRepository, IEmailService emailService)
        {
            _accountRepository = accountRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _transactionRepository = transactionRepository;
            _jewelryRepository = jewelryRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
            _sender = sender;
            _publisher = publisher;
            _paymentMethodRepository = paymentMethodRepository;
            _orderLogRepository = orderLogRepository;
            _orderFileServices = orderFileServices;
            _diamondRepository = diamondRepository;
            _emailService = emailService;
        }

        public async Task<Result<Order>> Handle(ProceedOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string? accountId, out var completeOrderRequestDto);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            var account = await _accountRepository.GetById(order.AccountId);
            var paymentMethods = await _paymentMethodRepository.GetAll();
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if (account == null)
                return Result.Fail(AccountErrors.AccountNotFoundError);
            if (!_orderService.IsProceedable(order.Status))
                return Result.Fail(OrderErrors.UnproceedableError);
            var orderItemQuery = _orderItemRepository.GetQuery();
            orderItemQuery = _orderItemRepository.QueryFilter(orderItemQuery, p => p.OrderId == order.Id);
            orderItemQuery = _orderItemRepository.QueryInclude(orderItemQuery, p => p.Jewelry);
            orderItemQuery = _orderItemRepository.QueryInclude(orderItemQuery, p => p.Diamond);
            var orderItems = orderItemQuery.ToList();
            //if (order.Status == OrderStatus.Pending)
            //{
            //    if(order.PaymentType == PaymentType.Payall)
            //    {
            //        Transaction trans = Transaction.CreateManualPayment(order.Id, $"Chuyển khoản từ tài khoản {order.Account?.FullName.FirstName} {order.Account?.FullName.LastName} cho đơn hàng {order.OrderCode}", _orderTransactionService.GetFullPaymentValueForOrder(order), TransactionType.Pay);
            //        trans.AppTransactionCode = "";
            //        trans.PaygateTransactionCode = "";
            //        await _transactionRepository.Create(trans);
            //        order.Status = OrderStatus.Processing;
            //        order.PaymentStatus = order.PaymentType == PaymentType.Payall ? PaymentStatus.Paid : PaymentStatus.Deposited;
            //    }
            //    else
            //    {
            //        decimal depositAmount = 0;
            //        if (order.IsCustomOrder)
            //            depositAmount = _orderTransactionService.GetCorrectAmountFromOrder(order);
            //        else
            //            depositAmount = _orderTransactionService.GetCorrectAmountFromOrder(order);
            //        Transaction trans = Transaction.CreateManualPayment(order.Id, $"Chuyển khoản từ tài khoản {order.Account?.FullName.FirstName} {order.Account?.FullName.LastName} cho đơn hàng {order.OrderCode}", depositAmount, TransactionType.Pay);
            //        trans.AppTransactionCode = "";
            //        trans.PaygateTransactionCode = "";
            //        await _transactionRepository.Create(trans);
            //        order.Status = OrderStatus.Processing;
            //        order.PaymentStatus = order.PaymentType == PaymentType.Payall ? PaymentStatus.Paid : PaymentStatus.Deposited;
            //    }

            //    await _orderRepository.Update(order);
            //    var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Processing);
            //    await _orderLogRepository.Create(log);
            //}
            //else 
            if (order.Status == OrderStatus.Processing)
            {
                //Change jewelry status if customize
                var items = order.Items;
                foreach (var item in items)
                {
                    if (order.CustomizeRequestId != null)
                    {
                        item.Jewelry.SetSold();
                        var getJewelryDiamond = await _diamondRepository.GetDiamondsJewelry(item.Jewelry.Id);
                        foreach (var diamond in getJewelryDiamond)
                        {
                            diamond.SetSold(diamond.DefaultPrice.Value, diamond.SoldPrice.Value);
                            await _diamondRepository.Update(diamond);
                        }
                        await _jewelryRepository.Update(item.Jewelry);
                    }
                    item.Status = OrderItemStatus.Prepared;
                }
                _orderItemRepository.UpdateRange(items);
                order.Status = OrderStatus.Prepared;
                order.FinishPreparedDate = DateTime.UtcNow;
                var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Prepared);
                var totalTransAmount = order.Transactions.Where(x => x.TransactionType == TransactionType.Pay)//|| x.TransactionType == TransactionType.Pay_Remain
                .Sum(x => x.TransactionAmount);
                _ = _emailService.SendOrderPreparedEmail(order, account, totalTransAmount, order.FinishPreparedDate.Value);
                await _orderLogRepository.Create(log);
            }
            else if (order.Status == OrderStatus.Prepared)
            {
                // nếu đơn hàng nhận tại quầy thì không cần phải giao hàng mà qua luôn success nếu đã thanh toán
                if (order.IsCollectAtShop)
                {
                    if (order.PaymentStatus != PaymentStatus.Paid)
                        return Result.Fail(OrderErrors.UnpaidError);
                    var items = order.Items;
                    foreach (var item in items)
                    {
                        item.Status = OrderItemStatus.Done;
                    }
                    order.Status = OrderStatus.Success;
                    var orderAtShopCompletelog = OrderLog.CreateByChangeStatus(order, OrderStatus.Success);
                    await _orderLogRepository.Create(orderAtShopCompletelog);
                    await _publisher.Publish(new OrderCompleteEvent(account.Id, order.Id, DateTime.UtcNow));
                }
                else
                {
                    //if(this is an order at shop then no delivere should be assigned, it should stayed here and proceed to success or cancelled, rejected)
                    if (order.DelivererId == null)
                        return Result.Fail(OrderErrors.NoDelivererAssignedError);
                    order.Status = OrderStatus.Delivering;
                    order.HasDelivererReturned = false;
                    var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Delivering);
                    await _orderLogRepository.Create(log);
                }
            }
            else if (order.Status == OrderStatus.Delivering)
            {
                var transactions = await _transactionRepository.GetByOrderId(order.Id);
                if (transactions != null)
                {
                    if (transactions.Any(p => p.Status == TransactionStatus.Verifying))
                        return Result.Fail(OrderErrors.Transfer.ExistVerifyingTransferError);
                    if (transactions.Any(p => p.Status == TransactionStatus.Invalid))
                        return Result.Fail(OrderErrors.Transfer.ExistInvalidTransferError);
                }
                if (accountId == null)
                    return Result.Fail(OrderErrors.NoDelivererToAssignError);
                if (order.DelivererId?.Value != accountId)
                    return Result.Fail(OrderErrors.OnlyDelivererAllowedError);
                if (order.PaymentStatus != PaymentStatus.Paid)
                    return Result.Fail(OrderErrors.UnpaidError);
                if (request.CompleteOrderRequestDto == null)
                {
                    return Result.Fail(OrderErrors.LackEvidenceToCompleteDeliver);
                }
                List<FileData> images = new();
                FileData video = null;
                if (request.CompleteOrderRequestDto.confirmImages == null || request.CompleteOrderRequestDto.confirmImages.Count() <= 0)
                    return Result.Fail(OrderErrors.LackEvidenceToCompleteDeliver);
                foreach (var image in request.CompleteOrderRequestDto.confirmImages)
                {
                    var fileName = image.FileName.Split('.')[0];
                    var fileExt = Path.GetExtension(image.FileName).Replace(".", "");
                    if (FileUltilities.IsImageFileContentType(image.ContentType) == false)
                        return Result.Fail(FileUltilities.Errors.NotCorrectImageFileType);
                    images.Add(new FileData(fileName, fileExt, image.ContentType, image.OpenReadStream()));
                }
                if (images.Count > 0)
                    await _orderFileServices.SaveOrderConfirmDeliveryImage(order, images);
                if (request.CompleteOrderRequestDto.confirmVideo != null)
                {
                    var videoFile = request.CompleteOrderRequestDto.confirmVideo;
                    var fileName = videoFile.FileName.Split('.')[0];
                    var fileExt = Path.GetExtension(videoFile.FileName).Replace(".", "");
                    if (FileUltilities.IsVideoFileContentType(videoFile.ContentType) == false)
                        return Result.Fail(FileUltilities.Errors.NotCorrectFileType("bằng chứng video thì là file mp4"));
                    video = new FileData(fileName, fileExt, videoFile.ContentType, videoFile.OpenReadStream());
                    await _orderFileServices.SaveOrderConfirmVideo(order, video);
                }
                var items = order.Items;
                foreach (var item in items)
                {
                    item.Status = OrderItemStatus.Done;
                }
                order.Status = OrderStatus.Success;
                var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Success);
                await _orderLogRepository.Create(log);
                //order.Raise(new OrderCompleteEvent(account.Id,order.Id, DateTime.UtcNow));
                await _publisher.Publish(new OrderCompleteEvent(account.Id, order.Id, DateTime.UtcNow));
            }
            else
            {
                return Result.Fail(OrderErrors.OrderStatusNotFoundError);
            }
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}

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
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Commands.ConfirmOrderTaken
{
    public record ConfirmEvidenceDto(IFormFile[]? confirmImages, IFormFile? confirmVideo);
    public record ConfirmOrderTakenFromShopCommand(string orderId, string? confirmerId, ConfirmEvidenceDto evidences) : IRequest<Result<Order>>;
    internal class ConfirmOrderTakenFromShopCommandHandler : IRequestHandler<ConfirmOrderTakenFromShopCommand, Result<Order>>
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

        public ConfirmOrderTakenFromShopCommandHandler(IAccountRepository accountRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, ITransactionRepository transactionRepository, IJewelryRepository jewelryRepository, IUnitOfWork unitOfWork, IOrderService orderService, IOrderTransactionService orderTransactionService, ISender sender, IPublisher publisher, IPaymentMethodRepository paymentMethodRepository, IOrderLogRepository orderLogRepository, IOrderFileServices orderFileServices, IDiamondRepository diamondRepository, IEmailService emailService)
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

        public async Task<Result<Order>> Handle(ConfirmOrderTakenFromShopCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();
            var order = await _orderRepository.GetById(OrderId.Parse(request.orderId));
            var account = await _accountRepository.GetById(order.AccountId);
            var paymentMethods = await _paymentMethodRepository.GetAll();
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if (account == null)
                return Result.Fail(AccountErrors.AccountNotFoundError);
            if(order.Status != Domain.Models.Orders.Enum.OrderStatus.Prepared)
                return Result.Fail(OrderErrors.UnproceedableError);

            if (request.evidences == null)
            {
                return Result.Fail(OrderErrors.LackEvidenceToCompleteDeliver);
            }
            if(order.PaymentStatus != PaymentStatus.Paid)
            {
                return Result.Fail(OrderErrors.UnfinishPayment);
            }
            List<FileData> images = new();
            FileData video = null;
            if (request.evidences.confirmImages == null || request.evidences.confirmImages.Count() <= 0)
                return Result.Fail(OrderErrors.LackEvidenceToCompleteDeliver);
            foreach (var image in request.evidences.confirmImages)
            {
                var fileName = image.FileName.Split('.')[0];
                var fileExt = Path.GetExtension(image.FileName).Replace(".", "");
                if (FileUltilities.IsImageFileContentType(image.ContentType) == false)
                    return Result.Fail(FileUltilities.Errors.NotCorrectImageFileType);
                images.Add(new FileData(fileName, fileExt, image.ContentType, image.OpenReadStream()));
            }
            if (images.Count > 0)
            {
                var uploadImageEvidenceResult = await _orderFileServices.SaveOrderConfirmDeliveryImage(order, images);
                if (uploadImageEvidenceResult.IsFailed)
                    return Result.Fail("khong thể luu chứng từ bây giờ, hãy thử lại sau");
            }
            if (request.evidences.confirmVideo != null)
            {
                var videoFile = request.evidences.confirmVideo;
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
            await _unitOfWork.SaveChangesAsync();
            await _publisher.Publish(new OrderCompleteEvent(account.Id, order.Id, DateTime.UtcNow));
            await _unitOfWork.CommitAsync();
            throw new NotImplementedException();
        }
    }
    
}

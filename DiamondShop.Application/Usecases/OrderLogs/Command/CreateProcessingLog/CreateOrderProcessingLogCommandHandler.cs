using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Commons;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Notifications;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DiamondShop.Application.Usecases.OrderLogs.Command.CreateProcessingLog
{
    public record CreateOrderProcessingLogCommand(string orderId, string message, IFormFile[]? images) : IRequest<Result<OrderLog>>;
    internal class CreateOrderProcessingLogCommandHandler : IRequestHandler<CreateOrderProcessingLogCommand, Result<OrderLog>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IBlobFileServices _blobFileServices;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IOrderFileServices _orderFileServices;
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationRepository _notificationRepository;

        public CreateOrderProcessingLogCommandHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IOrderLogRepository orderLogRepository, IBlobFileServices blobFileServices, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IOrderFileServices orderFileServices, IAccountRepository accountRepository, INotificationRepository notificationRepository)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _orderLogRepository = orderLogRepository;
            _blobFileServices = blobFileServices;
            _optionsMonitor = optionsMonitor;
            _orderFileServices = orderFileServices;
            _accountRepository = accountRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<Result<OrderLog>> Handle(CreateOrderProcessingLogCommand request, CancellationToken cancellationToken)
        {
            var loggingRule = _optionsMonitor.CurrentValue.LoggingRules;
            var parsedOrderId = OrderId.Parse(request.orderId);
            var getOrder = await _orderRepository.GetById(parsedOrderId);
            if (getOrder == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);

            List<OrderLog> orderLogs = await _orderLogRepository.GetOrderLogs(getOrder, cancellationToken);
            var orderedByTimeLogs = orderLogs.OrderBy(x => x.CreatedDate).ToList();
            var getProcessingParentLog = orderedByTimeLogs.FirstOrDefault(x => x.PreviousLogId == null && x.Status == OrderStatus.Processing);
            if (getProcessingParentLog == null)
                return Result.Fail(OrderErrors.LogError.ParentLogNotFound);

            var processingLog = OrderLog.CreateProcessingLog(getOrder, getProcessingParentLog, request.message);
            await _orderLogRepository.Create(processingLog);
            await _unitOfWork.SaveChangesAsync();
            var getUserAccount = await _accountRepository.GetById(getOrder.AccountId);
            var notification = Notification.CreateAccountMessage(getOrder, getUserAccount, "đã cập nhật trạng thái đơn của bạn", null);
            await _notificationRepository.Create(notification);
            await _unitOfWork.SaveChangesAsync();
            if (request.images != null)
            {
                List<FileData> imageDatas = new();
                foreach (var image in request.images)
                {
                    var ext = Path.GetExtension(image.FileName);
                    if (FileUltilities.IsImageFileExtension(ext) == false)
                        return Result.Fail(FileUltilities.Errors.NotCorrectImageFileType);
                    if (FileUltilities.IsImageFileContentType(image.ContentType) == false)
                        return Result.Fail(FileUltilities.Errors.NotCorrectImageFileType);
                    FileData fileData = new FileData(image.FileName, ext.Replace(".",""), image.ContentType, image.OpenReadStream());
                    imageDatas.Add(fileData);
                }
                var result = await _orderFileServices.UploadOrderLogImage(getOrder, processingLog, imageDatas.ToArray());
            }
            var getImages = await _orderFileServices.GetOrderLogImages(getOrder, processingLog);
            processingLog.LogImages = getImages.Value;
            return processingLog;
        }
    }
}

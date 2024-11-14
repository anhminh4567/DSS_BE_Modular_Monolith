using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace DiamondShop.Application.Usecases.OrderLogs.Command.CreateDeliveryLog
{
    public record CreateOrderDeliveryLogCommand(string orderId, string message, IFormFile[]? images) : IRequest<Result<OrderLog>>;
    internal class CreateOrderDeliveryLogCommandHandler : IRequestHandler<CreateOrderDeliveryLogCommand, Result<OrderLog>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IBlobFileServices _blobFileServices;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IOrderFileServices _orderFileServices;

        public CreateOrderDeliveryLogCommandHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IOrderLogRepository orderLogRepository, IBlobFileServices blobFileServices, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IOrderFileServices orderFileServices)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _orderLogRepository = orderLogRepository;
            _blobFileServices = blobFileServices;
            _optionsMonitor = optionsMonitor;
            _orderFileServices = orderFileServices;
        }

        public async Task<Result<OrderLog>> Handle(CreateOrderDeliveryLogCommand request, CancellationToken cancellationToken)
        {
            var loggingRule = _optionsMonitor.CurrentValue.LoggingRules;
            var parsedOrderId = OrderId.Parse(request.orderId);
            var getOrder = await _orderRepository.GetById(parsedOrderId);
            if (getOrder == null)
                return Result.Fail(new NotFoundError("Order not found"));

            List<OrderLog> orderLogs = await _orderLogRepository.GetOrderLogs(getOrder, cancellationToken);
            var orderedByTimeLogs = orderLogs.OrderBy(x => x.CreatedDate).ToList();
            var getDeliveringParentLog = orderedByTimeLogs.FirstOrDefault(x => x.PreviousLogId == null && x.Status == OrderStatus.Delivering);
            if (getDeliveringParentLog == null)
                return Result.Fail(new NotFoundError("No Delivering log found, this might be the order is not delivering yet to add extra log, or major logic error"));

            var deliveringLog = OrderLog.CreateDeliveringLog(getOrder, getDeliveringParentLog, request.message);
            await _orderLogRepository.Create(deliveringLog);
            await _unitOfWork.SaveChangesAsync();
            if (request.images != null)
            {
                List<FileData> imageDatas = new();
                foreach (var image in request.images)
                {
                    var ext = Path.GetExtension(image.FileName);
                    if (FileUltilities.IsImageFileExtension(ext) == false)
                        return Result.Fail(new ValidationError("Only image file is allowed"));
                    if (FileUltilities.IsImageFileContentType(image.ContentType) == false)
                        return Result.Fail(new ValidationError("Only image file is allowed"));
                    FileData fileData = new FileData(image.FileName, ext, image.ContentType, image.OpenReadStream());
                    imageDatas.Add(fileData);
                }
                var result = await _orderFileServices.UploadOrderLogImage(getOrder, deliveringLog, imageDatas.ToArray());
            }
            var getImages = await _orderFileServices.GetOrderLogImages(getOrder, deliveringLog);
            deliveringLog.LogImages = getImages.Value;
            return deliveringLog;
            throw new NotImplementedException();
        }
    }
}

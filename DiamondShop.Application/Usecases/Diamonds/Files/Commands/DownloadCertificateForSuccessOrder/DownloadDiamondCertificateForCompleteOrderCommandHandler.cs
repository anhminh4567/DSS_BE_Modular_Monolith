using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Files.Commands.DownloadCertificateForSuccessOrder
{
    public record DownloadDiamondCertificateForCompleteOrderCommand(string orderId, string diamondId): IRequest<Result<FileDownloadData>>;
    internal class DownloadDiamondCertificateForCompleteOrderCommandHandler : IRequestHandler<DownloadDiamondCertificateForCompleteOrderCommand, Result<FileDownloadData>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondFileService _diamondFileService;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public DownloadDiamondCertificateForCompleteOrderCommandHandler(IOrderRepository orderRepository, IDiamondRepository diamondRepository, IDiamondFileService diamondFileService, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _orderRepository = orderRepository;
            _diamondRepository = diamondRepository;
            _diamondFileService = diamondFileService;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<FileDownloadData>> Handle(DownloadDiamondCertificateForCompleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderId = OrderId.Parse(request.orderId);
            var diamondId = DiamondId.Parse(request.diamondId);
            var order = await _orderRepository.GetById(orderId);
            if(order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            var diamond = await _diamondRepository.GetById(diamondId);
            if(diamond == null)
                return Result.Fail(DiamondErrors.DiamondNotFoundError);
            if (order.Items.Any(x => x.DiamondId != null && x.DiamondId == diamondId) == false)
                return Result.Fail("kim cương không nằm trong đơn hàng này, bạn không được phép tải chứng chỉ");
            if (order.Status != Domain.Models.Orders.Enum.OrderStatus.Success)
                return Result.Fail("Đơn hàng chưa hoàn tất, bạn chỉ được tải chứng từ nếu hoàn tất đơn");

            throw new NotImplementedException();
        }
    }
}

using DiamondShop.Application.Dtos.Responses;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using FluentValidation;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Files.Commands
{
    public class GetOrCreateOrderInvoiceCommandValidator : AbstractValidator<GetOrCreateOrderInvoiceCommand>
    {
        public GetOrCreateOrderInvoiceCommandValidator()
        {
            RuleFor(x => x.orderId).NotEmpty();
        }
    }
    public record GetOrCreateOrderInvoiceCommand(string? orderId): IRequest<Result<MediaDto>>;
    public class GetOrCreateOrderInvoiceCommandHandler : IRequestHandler<GetOrCreateOrderInvoiceCommand, Result<MediaDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderFileServices _orderFileServices;
        private readonly IOrderRepository _orderRepository;
        private readonly IPdfService _pdfService;
        private readonly IMapper _mapper;

        public GetOrCreateOrderInvoiceCommandHandler(IUnitOfWork unitOfWork, IOrderFileServices orderFileServices, IOrderRepository orderRepository, IPdfService pdfService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _orderFileServices = orderFileServices;
            _orderRepository = orderRepository;
            _pdfService = pdfService;
            _mapper = mapper;
        }

        public async Task<Result<MediaDto>> Handle(GetOrCreateOrderInvoiceCommand request, CancellationToken cancellationToken)
        {
            var parsedId = OrderId.Parse(request.orderId);
            var order = await _orderRepository.GetById(parsedId);
            if(order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if (order.Status != Domain.Models.Orders.Enum.OrderStatus.Success)
                return Result.Fail(OrderErrors.NotComplete());
            var getFiles = await _orderFileServices.GetOrCreateOrderInvoice(order);
            var mapped = _mapper.Map<MediaDto>(getFiles.Value);
            return mapped;
        }
    }
}

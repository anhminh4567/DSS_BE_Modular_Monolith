using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Data;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.OrderItems.Command.Create
{
    public record CreateOrderItemCommand(OrderId OrderId, JewelryId? JewelryId, JewelryModelId? ModelId, List<DiamondId>? DiamondIds, string? EngravedText, string? EngravedFont) : IRequest<Result<OrderItem>>;
    internal class CreateOrderItemCommandHandler : IRequestHandler<CreateOrderItemCommand, Result<OrderItem>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderItemCommandHandler(IOrderItemRepository orderItemRepository, IUnitOfWork unitOfWork)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OrderItem>> Handle(CreateOrderItemCommand request, CancellationToken token)
        {
            throw new NotImplementedException();
            //DiamondId diamondId = null;
            //JewelryId jewelryId = null;
            //if (itemReq.modelId != null)
            //{

            //}
            //else
            //{
            //    var jewelry = jewelries.FirstOrDefault();
            //    if (jewelry is null)
            //        return Result.Fail(new ConflictError($"Jewelry {jewelry.Id} doesn't exist or has already been selected"));
            //    else if (!jewelry.IsActive || jewelry.IsSold)
            //        return Result.Fail(new ConflictError($"Jewelry {jewelry.Id} has already been sold"));
            //    else if (
            //        jewelry.Model.IsEngravable != (String.IsNullOrEmpty(itemReq.engravedText)))
            //        return Result.Fail(new ConflictError($"Jewelry {jewelry.Id} has already been sold"));

            //    if (itemReq.diamondId is not null)
            //    {
            //        var flagUnmatchedDiamonds = await _sender.Send(new CompareDiamondShapeCommand(jewelry.ModelId, attachedDiamonds));
            //        if (flagUnmatchedDiamonds.IsFailed) return Result.Fail(flagUnmatchedDiamonds.Errors);
            //    }
            //    jewelryId = jewelry.Id;
            //}
            //var orderItem = OrderItem.Create(orderId, jewelryId, diamondId,
            //itemReq.engravedText, itemReq.engravedFont, itemReq.purchasedPrice,
            //    itemReq.discountCode, itemReq.discountPercent, itemReq.promoCode, itemReq.promoPercent
            //    );
            //return orderItem;
        }
    }
}

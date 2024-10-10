using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using Mapster;

namespace DiamondShop.Application.Mappers
{
    public class OrderMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Order, OrderDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.AccountId, src => src.AccountId.Value)
                .Map(dest => dest.CustomizeRequestId, src => src.CustomizeRequestId.Value)
                .Map(dest => dest.ParentOrderId, src => src.ParentOrderId.Value)
                .Map(dest => dest.DeliveryPackageId, src => src.DeliveryPackageId.Value);

            config.NewConfig<OrderItem, OrderItemDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.OrderId, src => src.OrderId.Value)
                .Map(dest => dest.JewelryId, src => src.JewelryId.Value)
                .Map(dest => dest.DiamondId, src => src.DiamondId.Value);

            config.NewConfig<OrderLog, OrderLogDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.OrderId, src => src.OrderId.Value)
                .Map(dest => dest.PreviousLogId, src => src.PreviousLogId.Value);

            config.NewConfig<DeliveryPackage, DeliveryPackageDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.DelivererId, src => src.DelivererId.Value);
        }
    }
}

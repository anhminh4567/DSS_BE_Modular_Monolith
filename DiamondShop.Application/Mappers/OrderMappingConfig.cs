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
            config.NewConfig<Order, OrderDto>();

            config.NewConfig<OrderItem, OrderItemDto>();

            config.NewConfig<OrderLog, OrderLogDto>();
        }
    }
}

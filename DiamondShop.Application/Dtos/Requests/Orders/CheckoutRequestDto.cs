using DiamondShop.Application.Usecases.Orders.Commands.Create;

namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public record CheckoutOrderInfo(OrderRequestDto OrderRequestDto, List<OrderItemRequestDto> OrderItemRequestDtos);
    public record CheckoutRequestDto(BillingDetail BillingDetail, CheckoutOrderInfo CreateOrderInfo);
}

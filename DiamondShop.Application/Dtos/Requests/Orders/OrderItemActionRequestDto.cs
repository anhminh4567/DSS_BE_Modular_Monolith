namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public enum CompleteAction
    {
        Complete, RefundByShop, RefundByCustomer, ReplaceByShop, ReplaceByCustomer
    }
    public record OrderItemActionRequestDto(string ItemId, CompleteAction Action, OrderItemRequestDto? ReplacingItem);
}

﻿namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public enum CompleteAction
    {
        Refund, ReplaceByShop, ReplaceByCustomer
    }
    public record OrderItemActionRequestDto(string OrderItemId, CompleteAction Action);
}

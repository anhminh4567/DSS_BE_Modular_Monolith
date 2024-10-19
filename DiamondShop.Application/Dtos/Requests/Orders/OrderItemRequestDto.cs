using DiamondShop.Domain.Models.Warranties.Enum;

namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public record OrderItemRequestDto(
        string? JewelryId, string? DiamondId, 
        string? EngravedText, string? EngravedFont, 
        string WarrantyCode, WarrantyType WarrantyType);
}

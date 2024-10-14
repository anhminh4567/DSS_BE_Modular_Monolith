using DiamondShop.Domain.Models.Warranties.Enum;

namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public record OrderItemRequestDto(
        string? jewelryId, string? diamondId, 
        string? engravedText, string? engravedFont, 
        string? warrantyCode, WarrantyType? warrantyType);
}

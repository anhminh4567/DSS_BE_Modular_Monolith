using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Warranties.Enum;

namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public record OrderItemRequestDto(
        string? jewelryId, string? diamondId, 
        string? engravedText, string? engravedFont, 
        decimal purchasedPrice, 
        string? discountCode, int? discountPercent, 
        string? promoCode, int? promoPercent,
        string warrantyCode, decimal soldPrice, WarrantyType warrantyType);
}

using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.JewelryModels.Enum;

namespace DiamondShop.Application.Dtos.Requests.Jewelries
{
    public record JewelrySideDiamondRequestDto(float Carat, int Quantity, Color ColorMin, Color ColorMax, Clarity ClarityMin, Clarity ClarityMax, SettingType SettingType);

}

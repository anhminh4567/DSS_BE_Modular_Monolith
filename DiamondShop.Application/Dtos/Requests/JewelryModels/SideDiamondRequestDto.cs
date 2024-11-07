using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.JewelryModels.Enum;

namespace DiamondShop.Application.Dtos.Requests.JewelryModels
{
    public record SideDiamondRequestDto(string ShapeId, Color ColorMin, Color ColorMax, Clarity ClarityMin, Clarity ClarityMax, SettingType SettingType, float CaratWeight, int Quantity, bool IsLabDiamond);
}

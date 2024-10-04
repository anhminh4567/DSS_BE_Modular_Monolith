using DiamondShop.Domain.Models.JewelryModels.Enum;

namespace DiamondShop.Application.Dtos.Requests.JewelryModels
{
    public record MainDiamondRequestDto(List<MainDiamondShapeRequestDto> ShapeSpecs, SettingType SettingType, int Quantity);

}

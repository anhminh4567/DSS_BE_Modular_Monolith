using DiamondShop.Domain.Models.JewelryModels.Enum;

namespace DiamondShop.Application.Dtos.Requests.JewelryModels
{
    public record MainDiamondRequestDto(SettingType SettingType, int Quantity, List<MainDiamondShapeRequestDto> ShapeSpecs);

}

using DiamondShop.Domain.Models.JewelryModels.Enum;

namespace DiamondShop.Application.Dtos.Requests.JewelryModels
{
    public record JewelryModelRequestDto(
     string Name, string CategoryId, float? Width, float? Length,
     bool? IsEngravable, bool? IsRhodiumFinish,
     BackType? BackType, ClaspType? ClaspType, ChainType? ChainType
     );
}

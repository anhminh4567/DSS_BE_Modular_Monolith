using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Application.Dtos.Requests.Jewelries
{
    public record JewelryRequestDto(string ModelId, string SizeId, string MetalId, string SerialCode, bool AttachDiamond = false, ProductStatus Status = ProductStatus.Active)
    {

    };
}

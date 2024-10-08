using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Application.Dtos.Requests.Jewelries
{
    public record JewelryRequestDto(JewelryModelId ModelId, SizeId SizeId, MetalId MetalId, float Weight, string SerialCode);
}

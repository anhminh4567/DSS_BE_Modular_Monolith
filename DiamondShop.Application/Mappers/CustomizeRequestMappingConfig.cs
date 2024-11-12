using DiamondShop.Application.Dtos.Responses.CustomizeRequest;
using DiamondShop.Application.Dtos.Responses.Deliveries;
using DiamondShop.Application.Usecases.DeliveryFees.Commands.CalculateFee;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.DeliveryFees;
using Mapster;

namespace DiamondShop.Application.Mappers
{
    internal class CustomizeRequestMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CustomizeRequest, CustomizeRequestDto>();
        }
    }
}

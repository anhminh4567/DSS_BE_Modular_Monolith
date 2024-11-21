using DiamondShop.Application.Dtos.Responses.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests;
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

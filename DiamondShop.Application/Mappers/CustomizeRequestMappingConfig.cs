using DiamondShop.Application.Dtos.Responses.CustomizeRequest;
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

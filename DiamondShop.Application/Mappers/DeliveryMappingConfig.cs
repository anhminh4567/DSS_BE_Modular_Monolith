using DiamondShop.Application.Dtos.Responses.Deliveries;
using DiamondShop.Domain.Models.DeliveryFees;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Mappers
{
    internal class DeliveryMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<DeliveryFee, DeliveryFeeDto>();
        }
    }
}

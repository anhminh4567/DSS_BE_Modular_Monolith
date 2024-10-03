using DiamondShop.Application.Dtos.Responses.Accounts;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.RoleAggregate;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Mappers
{
    internal class AccountMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Account, AccountDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.FirstName, src => src.FullName.FirstName)
                .Map(dest => dest.LastName, src => src.FullName.LastName);
            config.NewConfig<AccountRole, AccountRoleDto>()
                .Map(dest => dest.Id, src => src.Id.Value);
            config.NewConfig<Address, AddressDto>()
                .Map(dest => dest.Id, src => src.Id.Value);


        }
    }
}

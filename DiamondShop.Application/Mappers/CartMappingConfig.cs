using DiamondShop.Application.Dtos.Responses.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Mappers
{
    public class CartMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CartItem, CartItemDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.JewelryId, src => (src.JewelryId == null) ? null : src.JewelryId.Value)
                .Map(dest => dest.DiamondId, src => (src.DiamondId == null) ? null : src.DiamondId.Value)
                .Map(dest => dest.JewelryModelId, src => (src.JewelryModelId == null) ? null : src.JewelryModelId.Value)
                .Map(dest => dest.SizeId, src => (src.SizeId == null) ? null : src.SizeId.Value)
                .Map(dest => dest.MetalId, src => (src.MetalId == null) ? null : src.MetalId.Value)
                //.Map(dest => dest.SideDiamondChoices, src => src.SideDiamondChoices)
                //.Ignore(config => config.SideDiamondChoices);
                //.Map(dest => dest.SideDiamondChoices, src => src.SideDiamondChoices);
                .Map(dest => dest.SideDiamondChoices, src => src.SideDiamondChoices.ToDictionary(kvp => kvp.Key.Value.ToString(), kvp => kvp.Value.Value.ToString()));
        }
    }
    
}

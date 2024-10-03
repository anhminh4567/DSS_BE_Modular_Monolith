using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Services.interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Carts.Commands.Add
{
    public record AddJewelry(string jewelryId,string? EngravedText, string? EngravedFont);
    public record AddDiamond(string diamondId);
    public record AddJewelryModel(string jewelryModelId, string sizeId, string metalId, Dictionary<string, string>? sideDiamondsChoices, string? EngravedText, string? EngravedFont);

    public record AddToCartCommand(string userId, AddJewelry? Jewelry, AddDiamond? Diamond, AddJewelryModel? JewelryModel) : IRequest<List<CartProduct>>;
    internal class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, List<CartProduct>>
    {
        private readonly ICartService _cartService;
        private readonly IDiamondServices _diamondServices;

        public AddToCartCommandHandler(ICartService cartService, IDiamondServices diamondServices)
        {
            _cartService = cartService;
            _diamondServices = diamondServices;
        }

        public async Task<List<CartProduct>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var accountId = AccountId.Parse(request.userId);
            CartItem item = null;
            if(request.Jewelry != null)
            {
                item = CartItem.CreateJewelry(JewelryId.Parse(request.Jewelry.jewelryId), request.Jewelry.EngravedText, request.Jewelry.EngravedFont);
            }
            else if (request.Diamond != null)
            {
                item = CartItem.CreateDiamond(DiamondId.Parse(request.Diamond.diamondId));

            }
            else if (request.JewelryModel != null)
            {
                Dictionary<SideDiamondReqId, SideDiamondOptId> mappedChoices = null;
                if(request.JewelryModel.sideDiamondsChoices != null)
                    mappedChoices = request.JewelryModel.sideDiamondsChoices.Select(kvp => new KeyValuePair<SideDiamondReqId,SideDiamondOptId>( SideDiamondReqId.Parse(kvp.Key),SideDiamondOptId.Parse(kvp.Value) )).ToDictionary();
                item = CartItem.CreateJewelryModel(JewelryModelId.Parse(request.JewelryModel.jewelryModelId), MetalId.Parse(request.JewelryModel.metalId), SizeId.Parse(request.JewelryModel.sizeId), mappedChoices);    
            }


            var addResult = await _cartService.AddProduct(accountId,item);
            return new();
        }
    }
}

using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Domain.Models.AccountAggregate.Entities
{
    public class CartItem : Entity<CartItemId>
    {
        public JewelryId? JewelryId { get; set; }
        public DiamondId? DiamondId { get; set; }
        public JewelryModelId? JewelryModelId { get; set; }
        public SizeId? SizeId { get; set; }
        public MetalId? MetalId { get; set; }
        public List<SideDiamondOptId> SideDiamondChoices { get; set; } = new();
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
        public static CartItem CreateJewelry(JewelryId jewelryId, string? engravedText, string? engravedFont)
        {
            return new CartItem { Id = CartItemId.Create(), JewelryId = jewelryId, EngravedText = engravedText, EngravedFont = engravedFont };
        }
        public static CartItem CreateJewelryModel (JewelryModelId jewelryModelId, MetalId metalId, SizeId? sizeId, List<SideDiamondOptId>? SideDiamondChoices , string? engravedText, string? engravedFont)
        {
           return new CartItem { Id = CartItemId.Create(), JewelryModelId = jewelryModelId, MetalId = metalId, SizeId = sizeId, SideDiamondChoices = SideDiamondChoices ,EngravedFont = engravedFont, EngravedText = engravedText};
        }
        public static CartItem CreateDiamond(DiamondId diamondId, JewelryModelId? jewelryModelId)
        {
            return new CartItem { Id = CartItemId.Create(), DiamondId = diamondId , JewelryModelId = jewelryModelId != null ? jewelryModelId : null  };
        }
    }
}

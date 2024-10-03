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
        public Dictionary<SideDiamondReqId, SideDiamondOptId>? SideDiamondChoices { get; set; }
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
        public static CartItem CreateJewelry(JewelryId jewelryId, string? engravedText, string? engravedFont)
        {
            return new CartItem { JewelryId = jewelryId, EngravedText = engravedText, EngravedFont = engravedFont };
        }
        public static CartItem CreateJewelryModel (JewelryModelId jewelryModelId, MetalId metalId, SizeId? sizeId, Dictionary<SideDiamondReqId, SideDiamondOptId>? SideDiamondChoices )
        {
           return new CartItem { JewelryModelId = jewelryModelId, MetalId = metalId, SizeId = sizeId, SideDiamondChoices = SideDiamondChoices };
        }
        public static CartItem CreateDiamond(DiamondId diamondId)
        {
            return new CartItem { DiamondId = diamondId };
        }
    }
}

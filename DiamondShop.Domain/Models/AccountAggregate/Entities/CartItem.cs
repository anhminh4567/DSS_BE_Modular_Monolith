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

namespace DiamondShop.Domain.Models.AccountAggregate.Entities
{
    public class CartItem : Entity<CartItemId>
    {
        public JewelryId? JewelryId { get; set; }
        public DiamondId? DiamondId { get; set; }
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
        
    }
}

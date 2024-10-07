using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Carts
{
    public class CartItemDto
    {
        public string Id { get; set; }
        public string? JewelryId { get; set; }
        public string? DiamondId { get; set; }
        public string? JewelryModelId { get; set; }
        public string? SizeId { get; set; }
        public string? MetalId { get; set; }
        public Dictionary<string, string>? SideDiamondChoices { get; set; } = new();
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
    }
}

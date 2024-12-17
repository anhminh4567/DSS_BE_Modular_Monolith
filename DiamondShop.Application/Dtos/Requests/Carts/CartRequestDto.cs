using DiamondShop.Application.Dtos.Requests.Accounts;
using DiamondShop.Domain.Models.Orders.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Requests.Carts
{
    public class CartRequestDto
    {
        public string? PromotionId { get; set; }
        public List<CartItemRequestDto> Items { get; set; } = new();
        public AddressRequestDto? UserAddress { get; set; }
        public string? AccountId { get; set; }
        public bool IsAtShopOrder { get; set; }
        public bool IsCustomOrder { get; set; } = false;
        //public string? PaymentMethodId { get; set; }
        public PaymentType? PaymentType { get; set; }
    }
}

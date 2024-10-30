using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Warranties.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Requests.Carts
{
    public class CartItemRequestDto
    {
        public string Id { get; set; }
        public string? JewelryId { get; set; }
        public string? DiamondId { get; set; }
        public string? JewelryModelId { get; set; }
        public string? SizeId { get; set; }
        public string? MetalId { get; set; }
        public List<string> SideDiamondChoices { get; set; } = new();
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
        public string? WarrantyCode { get; set; }
        public WarrantyType? WarrantyType { get; set; }
    }
    public class CartItemRequestDtoValidator : AbstractValidator<CartItemRequestDto>
    {
        public CartItemRequestDtoValidator()
        {
            RuleFor(x => x)
                .Must(x => ( x.JewelryId != null ||  x.DiamondId != null ||  x.JewelryModelId != null) )
                .WithMessage("At least one of JewelryId, DiamondId, or JewelryModelId must be provided.");
            
        }
    }
}

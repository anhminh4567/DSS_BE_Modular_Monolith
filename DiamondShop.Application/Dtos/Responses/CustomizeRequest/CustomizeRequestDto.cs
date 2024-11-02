using DiamondShop.Application.Dtos.Responses.Accounts;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Application.Dtos.Responses.CustomizeRequest
{
    public class CustomizeRequestDto
    {
        public string AccountId { get; set; }
        public AccountDto Account { get; set; }
        public string JewelryModelId { get; set; }
        public JewelryModelDto JewelryModel { get; set; }
        public string SizeId { get; set; }
        public SizeDto Size { get; set; }
        public MetalId MetalId { get; set; }
        public MetalDto Metal { get; set; }
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
        public string? Note { get; set; }
        public CustomizeRequestStatus Status { get; set; }
        public List<DiamondRequestDto> DiamondRequests { get; set; } = new();
        public string? SideDiamondId { get; set; }
        public SideDiamondOptDto? SideDiamond { get; set; }
    }
}

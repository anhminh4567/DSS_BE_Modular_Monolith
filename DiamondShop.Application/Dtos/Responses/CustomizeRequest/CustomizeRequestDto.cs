using DiamondShop.Application.Dtos.Responses.Accounts;
using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Application.Dtos.Responses.CustomizeRequest
{
    public class CustomizeRequestDto
    {
        public string Id { get; set; }
        public string RequestCode { get; set; }
        public string AccountId { get; set; }
        public AccountDto Account { get; set; }
        public string JewelryModelId { get; set; }
        public JewelryModelDto JewelryModel { get; set; }
        public string JewelryId { get; set; }
        public JewelryDto Jewelry { get; set; }
        public string SizeId { get; set; }
        public SizeDto Size { get; set; }
        public MetalId MetalId { get; set; }
        public MetalDto Metal { get; set; }
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public CustomizeRequestStatus Status { get; set; }
        public List<DiamondRequestDto> DiamondRequests { get; set; } = new();
        public string? SideDiamondId { get; set; }
        public SideDiamondOptDto? SideDiamond { get; set; }
    }
}

using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Domain.Models.CustomizeRequests
{
    public class CustomizeRequest : Entity<CustomizeRequestId>
    {
        public string RequestCode { get; set; }
        public AccountId AccountId { get; set; }
        public Account Account { get; set; }
        public JewelryId? JewelryId { get; set; }
        public Jewelry? Jewelry { get; set; }
        public JewelryModelId JewelryModelId { get; set; }
        public JewelryModel JewelryModel { get; set; }
        public SizeId SizeId { get; set; }
        public Size Size { get; set; }
        public MetalId MetalId { get; set; }
        public Metal Metal { get; set; }
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
        public string? Note { get; set; }   
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public CustomizeRequestStatus Status { get; set; } 
        public List<DiamondRequest> DiamondRequests { get; set; } = new();
        public SideDiamondOptId? SideDiamondId { get; set; }
        public SideDiamondOpt? SideDiamond { get; set; }
        public static CustomizeRequest CreatePending(AccountId accountId, JewelryModelId jewelryModelId, SizeId sizeId, MetalId metalId, SideDiamondOptId? sideDiamondOptId, string? engravedText, string? engravedFont, string? Note)
        {
            return new CustomizeRequest()
            {
                Id = CustomizeRequestId.Create(),
                AccountId = accountId,
                JewelryModelId = jewelryModelId,
                SizeId = sizeId,
                MetalId = metalId,
                SideDiamondId = sideDiamondOptId,
                EngravedText = engravedText,
                EngravedFont = engravedFont,
                Note = Note,
                Status = CustomizeRequestStatus.Pending,
                CreatedDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddMonths(CustomizeRequestRule.ExpiredMonthDuration)
            };
        }
        public static CustomizeRequest CreateRequesting(AccountId accountId, JewelryModelId jewelryModelId, SizeId sizeId, MetalId metalId, SideDiamondOptId? sideDiamondOptId, string? engravedText, string? engravedFont, string? Note)
        {
            return new CustomizeRequest()
            {
                Id = CustomizeRequestId.Create(),
                RequestCode = Utilities.GenerateRandomString(CustomizeRequestRule.RequestCodeLength),
                AccountId = accountId,
                JewelryModelId = jewelryModelId,
                SizeId = sizeId,
                MetalId = metalId,
                SideDiamondId = sideDiamondOptId,
                EngravedText = engravedText,
                EngravedFont = engravedFont,
                Note = Note,
                Status = CustomizeRequestStatus.Requesting,
                CreatedDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddMonths(CustomizeRequestRule.ExpiredMonthDuration)
            };
        }

        public void ResetExpiredDate()
        {
            ExpiredDate = DateTime.UtcNow.AddMonths(CustomizeRequestRule.AcceptedExpiredMonthDuration);
        }
    }
}

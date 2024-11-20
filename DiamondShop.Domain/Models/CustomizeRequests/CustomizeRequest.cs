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
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.ValueObjects;

namespace DiamondShop.Domain.Models.CustomizeRequests
{
    public class CustomizeRequest : Entity<CustomizeRequestId>
    {
        public string RequestCode { get; set; }
        public Order? Order { get; set; }
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
        public int Stage { get; set; }
        public static CustomizeRequest CreatePending(AccountId accountId, JewelryModelId jewelryModelId, SizeId sizeId, MetalId metalId, SideDiamondOptId? sideDiamondOptId, string? engravedText, string? engravedFont, string? note)
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
                Note = note,
                Stage = 0,
                Status = CustomizeRequestStatus.Pending,
                CreatedDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddMonths(CustomizeRequestRule.ExpiredMonthDuration),
            };
        }
        public static CustomizeRequest CreateRequesting(AccountId accountId, JewelryModelId jewelryModelId, SizeId sizeId, MetalId metalId, SideDiamondOptId? sideDiamondOptId, string? engravedText, string? engravedFont, string? note)
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
                Note = note,
                Stage = 6,
                Status = CustomizeRequestStatus.Requesting,
                CreatedDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddMonths(CustomizeRequestRule.ExpiredMonthDuration)
            };
        }
        public void SetPending()
        {
            Status = CustomizeRequestStatus.Pending;
            Stage = 0;
        }
        public void SetPriced()
        {
            Status = CustomizeRequestStatus.Priced;
            Stage = 3;
        }
        public void SetRequesting()
        {
            Status = CustomizeRequestStatus.Requesting;
            Stage = 6;
        }
        public void SetCustomerCancel()
        {
            if (Status == CustomizeRequestStatus.Pending)
                Stage = 1;
            else if (Status == CustomizeRequestStatus.Requesting)
                Stage = 7;
            else
                Stage = 10;
            Status = CustomizeRequestStatus.Customer_Cancelled;
        }
        public void SetCustomerReject()
        {
            Status = CustomizeRequestStatus.Customer_Rejected;
            Stage = 4;
        }
        public void SetShopReject()
        {
            if (Status == CustomizeRequestStatus.Pending)
                Stage = 2;
            else
                Stage = 8;
            Status = CustomizeRequestStatus.Shop_Rejected;
        }
        public void SetAccepted()
        {
            if (Status == CustomizeRequestStatus.Priced)
                Stage = 5;
            else
                Stage = 9;
            Status = CustomizeRequestStatus.Accepted;
        }
        public void ResetExpiredDate()
        {
            ExpiredDate = DateTime.UtcNow.AddMonths(CustomizeRequestRule.AcceptedExpiredMonthDuration);
        }
    }
}

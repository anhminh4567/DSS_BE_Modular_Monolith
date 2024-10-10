using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Models
{
    public record PaymentLinkRequest
    {
        public Account Account { get; set; }
        public Order Order { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string CallbackUrl { get; set; }
        public string ReturnUrl{ get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
    public record PaymentLinkResponse{
        public string PaymentUrl { get; set; }
        public string? QrCode { get; set; }
    }
    public record PaymentCallbackResponse
    {
        public string AppTransactionId { get; set; }
        public long PaygateTransactionId { get; set; }
        public long CreateOrderTime { get; set; }
        public string CustomerId { get; set; }
        public long Amount { get; set; }
        public long? PaygateHandleTimeStamp { get; set; }
        public string? PaygateUserId { get; set; }
        public long PaymentFee { get; set; } = 0;
    }
    public record PaymentMetadataBodyPerTransaction
    {
        public string TimeStampe { get; set; }
        public string GeneratedCode { get; set; }
        public string ForOrderId { get; set; }
        public string ForAccountId { get; set; }
        public string Description { get; set; }
    }

}

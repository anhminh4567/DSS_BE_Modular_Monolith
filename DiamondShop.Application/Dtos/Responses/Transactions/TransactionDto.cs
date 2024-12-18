using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Application.Dtos.Responses.Accounts;

namespace DiamondShop.Application.Dtos.Responses.Transactions
{
    public class TransactionDto
    {
        public string Id { get; set; }
        public string PayMethodId { get; set; }
        public PaymentMethodDto PayMethod { get; set; }
        public TransactionType TransactionType { get; set; }
        public string? InitDate { get; set; }
        public string? VerifiedDate { get; set; }
        public string Description { get; set; }
        public string PayDate { get; set; }
        public string AppTransactionCode { get; set; }
        public string PaygateTransactionCode { get; set; }
        public string TimeStampe { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal FineAmount { get; set; }
        public TransactionStatus Status { get; set; }
        public bool IsLegit { get; set; } 
        public bool IsManual { get; set; } 
        //For transfer
        public string? VerifierId { get; set; }
        public AccountDto? Verifier { get; set; }
        public MediaDto? Evidence { get; set; }
        public string? RefundedTransacId { get; set; }
        public TransactionDto? RefundedTransaction { get; set; }
        public List<OrderDto> Orders { get; set; } = new();
        public string? ShopBank { get; set; }
        public string? ShopAccount { get; set; }

    }
}

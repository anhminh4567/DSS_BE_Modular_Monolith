using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamondShop.Domain.Models.Transactions
{
    public class Transaction : Entity<TransactionId>, IAggregateRoot
    {
        public PaymentMethodId? PayMethodId { get; set; }
        public PaymentMethod? PayMethod { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public DateTime? InitDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
        //Transfer transactionCode
        public string? AppTransactionCode { get; set; }
        public string? PaygateTransactionCode { get; set; }
        public string TimeStamp { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal FineAmount { get; set; } = 0;
        [NotMapped]
        public decimal TotalAmount => TransactionAmount - FineAmount;
        public TransactionId? RefundedTransacId { get; set; }
        public Transaction? RefundedTransaction { get; set; }
        public OrderId? OrderId { get; set; }
        public Order Order { get; set; }
        public TransactionStatus Status { get; set; } = TransactionStatus.Verifying;
        public bool IsLegit { get; set; } = false;
        public bool IsManual { get; set; } = false;
        //For transfer
        public AccountId? VerifierId { get; set; }
        public Account? Verifier { get; set; }
        public Media? Evidence { get; set; }

        public static Transaction CreatePayment(PaymentMethodId paymentMethodId, OrderId orderId, string description, string appTransactionCode, string paygateTransactionCode, string timeStamp, decimal amount, DateTime paygatePaydate)
        {
            var id = TransactionId.Create();
            return new Transaction
            {
                Id = id,
                PayMethodId = paymentMethodId,
                TransactionType = TransactionType.Pay,
                Description = description,
                //ConfirmedPayDate = paygatePaydate,
                InitDate = DateTime.UtcNow,
                PaygateTransactionCode = paygateTransactionCode,
                AppTransactionCode = appTransactionCode,
                TimeStamp = timeStamp,
                TransactionAmount = amount,
                OrderId = orderId,
                FineAmount = 0,
                Status = TransactionStatus.Valid
            };
        }
        public static Transaction CreateRefund(PaymentMethodId paymentMethodId, OrderId orderId, TransactionId forTransactionId, string description, string appTransactionCode, string paygateTransactionCode, string timeStamp, decimal refundAmound, decimal fineAmount = 0)
        {
            var id = TransactionId.Create();
            return new Transaction
            {
                Id = id,
                OrderId = orderId,
                PayMethodId = paymentMethodId,
                TransactionType = TransactionType.Refund,
                Description = description,
                //ConfirmedPayDate = DateTime.UtcNow,
                InitDate = DateTime.UtcNow,
                PaygateTransactionCode = paygateTransactionCode,
                AppTransactionCode = appTransactionCode,
                TimeStamp = timeStamp,
                TransactionAmount = refundAmound,
                FineAmount = fineAmount,
                RefundedTransacId = forTransactionId,
                Status = TransactionStatus.Valid
            };
        }
        public static Transaction CreateManualPayment(OrderId orderId, string description, decimal amount, TransactionType type)
        {
            var dateTimeNow = DateTime.UtcNow;
            return new Transaction
            {
                Id = TransactionId.Create(),
                PayMethodId = null,
                TransactionType = type,
                Description = description,
                TransactionAmount = amount,
                OrderId = orderId,
                FineAmount = 0,
                VerifiedDate = dateTimeNow,
                TimeStamp = dateTimeNow.ToString(TransactionRule.TransactionTimeStamp),
                IsManual = true,
                Status = TransactionStatus.Verifying
            };
        }
        public static Transaction CreateManualRefund(OrderId orderId, AccountId verifierId, string transactionCode, string description, decimal amount)
        {
            var dateTimeNow = DateTime.UtcNow;
            return new Transaction
            {
                Id = TransactionId.Create(),
                VerifierId = verifierId,
                PayMethodId = null,
                AppTransactionCode = transactionCode,
                TransactionType = TransactionType.Refund,
                Description = description,
                TransactionAmount = amount,
                OrderId = orderId,
                VerifiedDate = dateTimeNow,
                TimeStamp = dateTimeNow.ToString(TransactionRule.TransactionTimeStamp),
                //FineAmount = finedAmount,
                IsManual = true,
                Status = TransactionStatus.Verifying
            };
        }

        private Transaction() { }
        public void Verify(AccountId verifierId, string transactionCode)
        {
            VerifierId = verifierId;
            AppTransactionCode = transactionCode;
            Status = TransactionStatus.Valid;
            VerifiedDate = DateTime.UtcNow;
            
        }

        //public void VerifyZalopay(string transactionCode, string paygateTransactionCode, string timeStamp)
        //{
        //    AppTransactionCode = transactionCode;
        //    PaygateTransactionCode = paygateTransactionCode;
        //    Status = TransactionStatus.Valid;
        //    VerifiedDate = DateTime.UtcNow;
        //    TimeStamp = timeStamp;
        //}
        public void Cancel()
        {
            Status = TransactionStatus.Invalid;
            VerifiedDate = null;
            AppTransactionCode = null;
            PaygateTransactionCode = null;
        }
    }
}

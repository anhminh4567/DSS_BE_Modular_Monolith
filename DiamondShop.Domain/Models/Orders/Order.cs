using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamondShop.Domain.Models.Orders
{
    public class Order : Entity<OrderId>, IAggregateRoot
    {
        public string OrderCode { get; set; }
        public AccountId AccountId { get; set; }
        public Account? Account { get; set; }
        public AccountId? DelivererId { get; set; }
        public Account? Deliverer { get; set; }
        public CustomizeRequestId? CustomizeRequestId { get; set; }
        public CustomizeRequest? CustomizeRequest { get; set; }
        public PromotionId? PromotionId { get; set; }
        public Promotion? Promotion { get; set; }
        public string? PromotionCode { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime ExpectedDate { get; set; } = DateTime.Now.AddDays(OrderRules.ExpectedDeliveryDate).ToUniversalTime();
        public DateTime? ShippedDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? CancelledReason { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentMethodId? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DepositFee { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal OrderAmountSaved { get; set; } = 0;
        public decimal UserRankAmountSaved { get; set; } = 0;
        public decimal TotalRefund { get; set; } = 0;
        public decimal TotalFine { get; set; } = 0;
        //tobe removed
        public decimal OrderSavedAmount { get; set; } = 0;
        public string ShippingAddress { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public List<OrderLog> Logs { get; set; } = new();
        public List<Transaction> Transactions { get; set; } = new();
        public string? Note { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? ShipFailedDate{ get; set; }
        public int ShipFailedCount { get; set; } = 0;
        [NotMapped]
        public bool IsCustomOrder { get => CustomizeRequestId != null; }
        public void AddTransaction(Transaction transactionTypePay) 
        {
            if(transactionTypePay.TransactionType != Models.Transactions.Enum.TransactionType.Pay)
                throw new InvalidOperationException("Transaction type must be Pay");
            Transactions.Add(transactionTypePay);
        }
        public void AddRefund(Transaction transactionTypeRefund)
        {
            if (transactionTypeRefund.TransactionType != Models.Transactions.Enum.TransactionType.Refund)
                throw new InvalidOperationException("Transaction type must be Refund");
            TotalRefund += transactionTypeRefund.TotalAmount;
            TotalFine += transactionTypeRefund.FineAmount;
            Transactions.Add(transactionTypeRefund);
        }

        public Order() { }
        public static Order Create(AccountId accountId, PaymentType paymentType, PaymentMethodId paymentMethodId,
            decimal totalPrice, decimal shippingFee, decimal depositFee, string shippingAddress, CustomizeRequestId? customizeRequestId = null,
            Promotion? promotion = null, decimal orderSavedAmount = 0 , decimal UserRankAmountSaved = 0, DateTime? expiredDate = null, OrderId givenId = null)
        {
            return new Order()
            {
                Id = givenId is null ? OrderId.Create() : givenId,
                OrderCode = Utilities.GenerateRandomString(OrderRules.OrderCodeLength),
                AccountId = accountId,
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                CustomizeRequestId = customizeRequestId,
                PromotionId = promotion == null ? null : promotion.Id,
                PromotionCode = promotion == null ? null : promotion.PromoCode,
                PaymentType = paymentType,
                TotalPrice = totalPrice,
                ShippingFee = shippingFee,
                ShippingAddress = shippingAddress,
                OrderAmountSaved = orderSavedAmount,
                UserRankAmountSaved = UserRankAmountSaved,
                PaymentMethodId = paymentMethodId,
                DepositFee = depositFee,
                ExpiredDate = expiredDate,
            };
        }
        public void Deposit(Transaction depositedTransaction)
        {
            if(depositedTransaction.TotalAmount >= TotalPrice)
                throw new InvalidOperationException("Transaction deposit must be less then the order, this is not deposit transaction");
            AddTransaction(depositedTransaction);
            Status = OrderStatus.Processing;
            PaymentStatus = PaymentStatus.Deposited;
            Items.ForEach(p => p.Status = OrderItemStatus.Pending);
        }
        public void PayAll(Transaction payAllTransaction)
        {
            if (payAllTransaction.TotalAmount < TotalPrice)
                throw new InvalidOperationException("Transaction pay all must be equal the order, this is not pay all transaction");
            AddTransaction(payAllTransaction);
            Status = OrderStatus.Processing;
            PaymentStatus = PaymentStatus.Paid;
            Items.ForEach(p => p.Status = OrderItemStatus.Pending);
        }
        public void PayRemainingForDepositOrder(Transaction payRemainingTransaction)
        {
            var totalDeposited = Transactions.Where(p => p.TransactionType == TransactionType.Pay).Sum(x => x.TotalAmount);
            var remaining = TotalPrice - totalDeposited;
            if (payRemainingTransaction.TotalAmount < remaining)
                throw new InvalidOperationException("Transaction for the remaining is not valid");
            AddTransaction(payRemainingTransaction);
            Status = OrderStatus.Success;
            PaymentStatus = PaymentStatus.Paid;
            Items.ForEach(p => p.Status = OrderItemStatus.Done);
        }
        public void SetExpired(DateTime expiredTime)
        {
            CancelledDate = expiredTime.ToUniversalTime();
            ExpiredDate = CancelledDate;
            Status = OrderStatus.Cancelled;
            CancelledReason = "quá hạn thanh toán, vui lòng mua đơn mới ";
        }

        public decimal GetOrderItemPriceOnly()
        {
            if (Items.Count == 0)
                throw new Exception("make sure there are product item included") ;
            var totalWarrantyPrice = Items.Sum(x => x.WarrantyPrice);
            return TotalPrice - ShippingFee + OrderAmountSaved + UserRankAmountSaved - totalWarrantyPrice;
        }
        public decimal GetOrderExcludeShipping()
        {
            return TotalPrice - ShippingFee;
        }
    }
}

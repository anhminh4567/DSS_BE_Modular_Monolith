using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Notifications.ValueObjects;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders
{
    public class Order : Entity<OrderId>, IAggregateRoot
    {
        public AccountId AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? CancelledReason { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalRefund { get; set; }
        public decimal TotalFine { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItem> Items { get; set; }
        public List<OrderLog> Logs { get; set; }
        public TransactionId? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }

        public OrderId? ParentOrderId { get; set; } // for replacement order
        public DeliveryPackageId? DeliveryPackageId { get; set; }

        public Order() { }
    }
}

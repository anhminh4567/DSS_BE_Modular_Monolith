using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Transactions
{
    public class Transaction : Entity<TransactionId>, IAggregateRoot
    {
        public PaymentMethodId PayMethodId { get; set; }
        public PaymentMethod PayMethod { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Note { get; set; }
        public DateTime PayDate { get; set; }
        public string TransactionCode { get; set; }
        public decimal TransactionAmount { get; set; }
        public TransactionId? RefundedTransacId { get; set; }
        public Transaction? RefundedTransac { get; set; }
        public List<Order> Orders { get; set; } = new ();
    }
}

using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.Enum;

namespace DiamondShop.Application.Dtos.Responses.Transactions
{
    public class TransactionDto
    {
        public string PayMethodId { get; set; }
        public PaymentMethod PayMethod { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public string PayDate { get; set; }
        public string AppTransactionCode { get; set; }
        public string PaygateTransactionCode { get; set; }
        public string TimeStampe { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal FineAmount { get; set; }

        public string? RefundedTransacId { get; set; }
        public TransactionDto? RefundedTransaction { get; set; }
        public List<OrderDto> Orders { get; set; } = new();

    }
}

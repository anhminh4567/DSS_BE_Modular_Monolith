using DiamondShop.Application.Dtos.Responses.Accounts;
using DiamondShop.Application.Dtos.Responses.Transactions;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders.Enum;

namespace DiamondShop.Application.Dtos.Responses.Orders
{
    public class OrderDto
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public AccountDto? Account { get; set; }
        public string? DelivererId { get; set; }
        public AccountDto? Deliverer { get; set; }
        public string? CustomizeRequestId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? CancelledReason { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentType PaymentStatus { get; set; }
        public string? PaymentMethodId { get; set; }
        public PaymentMethodDto? PaymentMethod { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalRefund { get; set; }
        public decimal TotalFine { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public List<OrderLogDto> Logs { get; set; }
        public string? TransactionId { get; set; }
        public TransactionDto? Transaction { get; set; }
        public string OrderCode { get; set; }
        public string? ParentOrderId { get; set; } // for replacement order
        public string? DeliveryPackageId { get; set; }


    }
}

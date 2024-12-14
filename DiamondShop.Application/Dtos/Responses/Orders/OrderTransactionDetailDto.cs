using DiamondShop.Application.Dtos.Responses.Transactions;
using DiamondShop.Application.Usecases.AdminConfigurations.Orders.Commands;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Orders.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Orders
{
    public class OrderTransactionDetailDto
    {
        public List<TransactionDto> Transactions { get; set; }
        public OrderPaymentRules PaymentRules { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentType PaymentType { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsShopFault { get; set; }
        public bool IsRefundable { get; set; }
        public decimal ExpectedRefundAmount { get; set; }
        public decimal ExpectedPayAmount { get; set; }
        
    }
}

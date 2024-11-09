using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Transactions.Entities
{
    public class PaymentMethod : Entity<PaymentMethodId>
    {
        public static PaymentMethod BANK_TRANSFER = PaymentMethod.Create("BANK_TRANSFER", PaymentMethodId.Parse("1"));
        public static PaymentMethod ZALOPAY = PaymentMethod.Create("ZALOPAY", PaymentMethodId.Parse("2"));
        public string MethodName { get; set; }
        public string? MethodThumbnailPath { get; set; }
        public bool Status { get; set; }
        public static PaymentMethod Create(string methodName, string givenId = null)
        {
            return new PaymentMethod
            {
                MethodName = methodName,
                Status = true,
                Id = givenId == null ? PaymentMethodId.Create() : PaymentMethodId.Parse(givenId),
            };
        }
        internal static PaymentMethod Create(string methodName, PaymentMethodId id)
        {
            return new PaymentMethod
            {
                MethodName = methodName,
                Status = true,
                Id = id
            };
        }
        public PaymentMethod() { }
    }
    public static class PaymentMethodHelper
    {

        public static List<PaymentMethod> AllMethods = new List<PaymentMethod> { PaymentMethod.BANK_TRANSFER, PaymentMethod.ZALOPAY };
        public static string GetMethodName(PaymentMethod method)
        {
            ArgumentNullException.ThrowIfNull(method);
            if (method.Id == PaymentMethod.BANK_TRANSFER.Id)
                return "Chuyển khoản qua cho shop qua ngân hàng";
            if (method.Id == PaymentMethod.ZALOPAY.Id)
                return "Thanh toán qua ZaloPay";
            return "Không xác định";
        }
    }
}

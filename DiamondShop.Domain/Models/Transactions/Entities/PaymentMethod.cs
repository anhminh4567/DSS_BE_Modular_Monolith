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
        public string MethodName { get; set; }
        public string? MethodThumbnailPath { get; set; }
        public bool Status { get; set; }
        public static PaymentMethod Create(string methodName,string givenId = null)
        {
            return new PaymentMethod
            {
                MethodName = methodName,
                Status = true,
                Id = givenId == null ? PaymentMethodId.Create() : PaymentMethodId.Parse(givenId),
            };
        }
        public PaymentMethod() { }
    }
}

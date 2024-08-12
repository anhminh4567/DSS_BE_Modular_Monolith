using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Common.ValueObjects
{
    public record Money
    {
        public decimal Value { get; private set; } = 0;
        public string Currency { get; private set; }
        protected Money(string currency, decimal amount)
        {
            if (currency == null)
                throw new ArgumentNullException();
            switch (currency)
            {
                case "VND":
                    Value = amount;
                    Currency = "VND";
                    break;
                case "USD":
                    Value = amount * (decimal)22.0;
                    Currency = "USD";
                    break;
                default:
                    throw new ArgumentException("currency not found");
            }

        }
        public static Money CreateVnd(decimal amountVND)
        {
            return new Money("VND", amountVND);
        }
        public static Money Create(string currency, decimal amountVND)
        {
            return new Money(currency, amountVND);
        }
        private Money()
        {

        }
    }
}

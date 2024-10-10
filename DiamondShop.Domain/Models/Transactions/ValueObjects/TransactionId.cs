using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Transactions.ValueObjects
{
    public record TransactionId(string Value)
    {
        public static TransactionId Parse(string id)
        {
            return new TransactionId(id) { Value = id };
        }
        public static TransactionId Create()
        {
            return new TransactionId(Ulid.NewUlid().ToString());
        }
    }
}

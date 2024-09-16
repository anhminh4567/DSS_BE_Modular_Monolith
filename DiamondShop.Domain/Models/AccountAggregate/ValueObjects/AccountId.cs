using DiamondShop.Domain.Models.AccountRoleAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.AccountAggregate.ValueObjects
{
    public record AccountId (string Value)
    {
        public static AccountId Parse(string id)
        {
            return new AccountId(id) { Value = id };
        }
        public static AccountId Create()
        {
            return new AccountId(Guid.NewGuid().ToString());
        }
    }
}

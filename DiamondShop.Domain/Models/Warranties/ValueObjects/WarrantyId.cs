using DiamondShop.Domain.Models.Transactions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Warranties.ValueObjects
{
    public record WarrantyId(string Value )
    {
        public static WarrantyId Parse(string id)
        {
            return new WarrantyId(id) { Value = id };
        }
        public static WarrantyId Create()
        {
            return new WarrantyId(Guid.NewGuid().ToString());
        }
    }
}

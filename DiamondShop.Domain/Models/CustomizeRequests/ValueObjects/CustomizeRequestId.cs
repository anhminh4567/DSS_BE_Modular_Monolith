using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.CustomizeRequests.ValueObjects
{
    public record CustomizeRequestId (string Value  )
    {
        public static CustomizeRequestId Parse(string id)
        {
            return new CustomizeRequestId(id) { Value = id };
        }
        public static CustomizeRequestId Create()
        {
            return new CustomizeRequestId(Guid.NewGuid().ToString());
        }
    }
}

using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.ValueObjects
{
    public record OrderLogId (string Value)
    {
        public static OrderLogId Parse(string id)
        {
            return new OrderLogId(id) { Value = id };
        }
        public static OrderLogId Create()
        {
            return new OrderLogId(Ulid.NewUlid().ToString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.ValueObjects
{
    public record GiftId(string Value)
    {
        public static GiftId Parse(string id)
        {
            return new GiftId(id) { Value = id };
        }
        public static GiftId Create()
        {
            return new GiftId(Guid.NewGuid().ToString());
        }
    }
}

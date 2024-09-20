using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.ValueObjects
{
    public record PromoReqId(string Value)
    {
        public static PromoReqId Parse(string id)
        {
            return new PromoReqId(id) { Value = id };
        }
        public static PromoReqId Create()
        {
            return new PromoReqId(Guid.NewGuid().ToString());
        }
    }
}

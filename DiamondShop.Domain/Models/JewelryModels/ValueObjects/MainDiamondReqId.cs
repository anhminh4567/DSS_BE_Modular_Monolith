using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.ValueObjects
{
    public record MainDiamondReqId(string Value)
    {
        public static MainDiamondReqId Parse(string id)
        {
            return new MainDiamondReqId(id) { Value = id };
        }
        public static MainDiamondReqId Create()
        {
            return new MainDiamondReqId(Ulid.NewUlid().ToString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.CustomizeRequests.ValueObjects
{
    public record DiamondRequestId (string Value)
    {
        public static DiamondRequestId Parse(string id)
        {
            return new DiamondRequestId(id) { Value = id };
        }
        public static DiamondRequestId Create()
        {
            return new DiamondRequestId(DateTime.UtcNow.Ticks.ToString().Substring(5));
        }
    }
}

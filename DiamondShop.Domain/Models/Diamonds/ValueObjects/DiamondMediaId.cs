using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds.ValueObjects
{
    public record DiamondMediaId (string Value)
    {
        public static DiamondMediaId Parse(string id) 
        { 
            return new DiamondMediaId(id) { Value = id };
        }
        public static DiamondMediaId Create()
        {
            return new DiamondMediaId(Guid.NewGuid().ToString());
        }
    }
}

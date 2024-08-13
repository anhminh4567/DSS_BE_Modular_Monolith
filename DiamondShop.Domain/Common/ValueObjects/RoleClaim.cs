using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Common.ValueObjects
{
    public record RoleClaim (string id, string name)
    {

        public static RoleClaim Parse(string id, string name)
        {
            return new RoleClaim(id,name);
        }
    }
}

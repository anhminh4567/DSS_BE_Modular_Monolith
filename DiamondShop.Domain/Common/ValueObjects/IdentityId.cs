using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Common.ValueObjects
{
    public record IdentityId
    {
        public string Value { get; set; }
        public IdentityId(string id)
        {
            if (id is null)
                throw new ArgumentNullException();
            Value = id;
        }
        public static IdentityId Parse(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new IdentityId("");
            return new IdentityId(id);
        }
    }
}

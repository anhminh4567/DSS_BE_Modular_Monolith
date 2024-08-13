using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.StaffAggregate.ValueObjects
{
    public record StaffId(string value)
    {
        public static StaffId Create()
        {
            return new StaffId(Guid.NewGuid().ToString());
        }
        public static StaffId Parse(string id)
        {
            return new StaffId(id);
        }

    }
}

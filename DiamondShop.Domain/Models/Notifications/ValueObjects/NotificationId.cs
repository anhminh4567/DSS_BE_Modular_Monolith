using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Notifications.ValueObjects
{
    public record NotificationId(string Value)
    {
        public static NotificationId Parse(string id)
        {
            return new NotificationId(id) { Value = id };
        }
        public static NotificationId Create()
        {
            return new NotificationId(Guid.NewGuid().ToString());
        }
    }
}

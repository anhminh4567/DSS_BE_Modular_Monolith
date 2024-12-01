using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Options
{
    public class InAppNotificationOptions
    {
        public static string Section = "InAppNotification";
        public int PublicMessageExpiredHour { get; set; } = 1;
        public int AccountMessageExpiredHour { get; set; } = 8;
        public int ShopMessageExpiredHour { get; set; } = 24;
    }
}

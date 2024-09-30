using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public static class DateTimeFormatingRules
    {
        // this is global format, so that frontend can easily send to server without mis-
        public static string DateTimeFormat { get; set; } = "dd-MM-yyyy HH:mm:ss";
        public static string DateFormat { get; set; } = "dd-MM-yyyy";
        public static string TimeFormat { get; set; } = "HH:mm:ss";
        //public static string PromotionFormat_ServerSide = "dd-MM-yyyy HH";
    }
}

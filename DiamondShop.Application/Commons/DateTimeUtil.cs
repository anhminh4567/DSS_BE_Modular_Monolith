using DiamondShop.Domain.BusinessRules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons
{
    internal class DateTimeUtil
    {
        public static bool BeAValidDate(string dateString)
        {
            // Simple date format validation
            return DateTime.TryParseExact(dateString, DateTimeFormatingRules.DateTimeFormat, null, DateTimeStyles.None, out _);
        }
    }
}

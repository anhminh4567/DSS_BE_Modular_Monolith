using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Currencies.RapidApis.Models
{
    public class RapidApiConvertResponseDetail
    {
        public string Date { get; set; }
        public RapidApiInfo Info { get; set; }
        public RapidApiQuery Query { get; set; }
        public decimal Result { get; set; }
        public bool Success { get; set; }
    }

    public class RapidApiInfo
    {
        public decimal Rate { get; set; }
        public long Timestamp { get; set; }
    }

    public class RapidApiQuery
    {
        public decimal Amount { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}

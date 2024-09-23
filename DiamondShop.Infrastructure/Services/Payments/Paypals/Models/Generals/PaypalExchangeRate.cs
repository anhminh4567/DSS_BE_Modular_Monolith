using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals
{
    public class PaypalExchangeRate
    {
        public string source_currency {  get; set; }
        public string target_currency { get; set; }
        public string value { get; set; }

    }
}

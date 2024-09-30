using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals.AmountBreakdown
{

    public class PaypalBreakdown
    {
        public PaypalMoney item_total { get; set; }
        public PaypalMoney shipping { get; set; }
        public PaypalMoney handling { get; set; }
        public PaypalMoney tax_total { get; set; }
        public PaypalMoney insurance { get; set; }
        public PaypalMoney shipping_discount { get; set; }
        public PaypalMoney discount { get; set; }
    }
}


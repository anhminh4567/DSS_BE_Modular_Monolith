using DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models
{
    public class PaypalCreateOrderBody
    {
        public List<PaypalPurchaseUnitItem> purchase_units { get; set; }
        public string intent { get; set; }
    }

}

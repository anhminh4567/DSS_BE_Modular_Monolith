using DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models
{
    public class PaypalOrderDetail
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Intent { get; set; }
        public PaypalPaymentSource payment_source { get; set; }
        public List<PaypalPurchaseUnitItem> purchase_units { get; set; }
        public PaypalPayer payer { get; set; }
        public List<PaypalLink> links { get; set; }
    }
}

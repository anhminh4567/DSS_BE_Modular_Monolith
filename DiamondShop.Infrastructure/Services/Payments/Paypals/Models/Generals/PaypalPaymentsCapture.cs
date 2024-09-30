using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals.AmountBreakdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals
{
    public class PaypalPaymentsCapture
    {
        public string id { get; set; }
        public string status { get; set; }
        public PaypalAmount amount { get; set; }
        //public SellerProtection seller_protection { get; set; }
        public bool final_capture { get; set; }
        public string disbursement_mode { get; set; }
        public PaypalSellerReceivableBreakdown seller_receivable_breakdown { get; set; }
        public string create_time { get; set; }
        public string update_time { get; set; }
        public List<PaypalLink> links { get; set; }
    }
}

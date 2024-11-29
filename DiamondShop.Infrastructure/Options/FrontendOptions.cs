using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Options
{
    public class FrontendOptions
    {
        public static string Section = "Frontend";
        public string BaseUrl { get; set; }
        public string SuccessPaymentUrl { get; set; }
        public string FailedPaymentUrl { get; set; }
        public string ConfirmEmailSuccessUrl { get; set; }
        public string ConfirmEmailFailedUrl { get; set; }
        public string OrderDetailUrl { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Constants
{
    public  class PaypalRefundStatus
    {
        public const string CANCELLED = "CANCELLED";
        public const string FAILED = "FAILED";
        public const string PENDING = "PENDING";
        public const string COMPLETED = "COMPLETED";

    }
}

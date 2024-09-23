using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Constants
{
    public  class PaypalOrderStatus
    {
        public const string CREATED = "CREATED";
        public const string APPROVED = "APPROVED";
        public const string COMPLETED = "COMPLETED";
        public const string PAYER_ACTION_REQUIRED = "PAYER_ACTION_REQUIRED";

    }
}

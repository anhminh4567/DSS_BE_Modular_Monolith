using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals
{
    public class PaypalCreateOrderResult
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public List<PaypalLink> Links { get; set; } = new();
    }
}

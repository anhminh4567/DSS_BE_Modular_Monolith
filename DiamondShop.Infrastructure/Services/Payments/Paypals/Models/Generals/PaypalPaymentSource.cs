using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals
{
    public class PaypalPaymentSource
    {
        [JsonProperty("paypal")]
        public PaypalPayment? PayPal { get; set; }
    }
    public class PaypalPayment
    {
        public PaypalName Name { get; set; }
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }
        public string AccountId { get; set; }
    }
    public class PaypalName
    {
        [JsonProperty("given_name")]
        public string GivenName { get; set; }
        public string Surname { get; set; }
    }

}

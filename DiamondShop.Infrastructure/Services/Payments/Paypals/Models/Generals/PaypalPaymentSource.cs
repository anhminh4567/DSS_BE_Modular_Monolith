using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals
{
    public class PaypalPaymentSource
    {
        [JsonPropertyName("paypal")]
        public PaypalPayment? PayPal { get; set; }
    }
    public class PaypalPayment
    {
        public PaypalName Name { get; set; }
        [JsonPropertyName("email_address")]
        public string EmailAddress { get; set; }
        public string AccountId { get; set; }
    }
    public class PaypalName
    {
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }
        [JsonPropertyName("surname")]
        public string Surname { get; set; }
    }

}

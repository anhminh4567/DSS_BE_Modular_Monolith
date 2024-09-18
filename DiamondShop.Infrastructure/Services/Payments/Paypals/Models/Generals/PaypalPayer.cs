using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals
{
    public class PaypalPayer
    {
        public PaypalName Name { get; set; }
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }
        public string AccountId { get; set; }
    }
}

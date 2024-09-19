using DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals;
using DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals.AmountBreakdown;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models
{
    public class PaypalPaymentDetail
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string TransactionId { get; set; }
        public string status { get; set; }
        public PaypalMoney amount { get; set; }
        public PaypalSellerReceivableBreakdown seller_receivable_breakdown { get; set; }
        public string invoice_id { get; set; }
        public string create_time { get; set; }
        public string update_time { get; set; }
        public List<PaypalLink> links { get; set; }
    }
}

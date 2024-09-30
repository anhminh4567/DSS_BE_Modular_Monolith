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
    public class PaypalRefundDetail
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string TransactionId { get; set; }
        public PaypalMoney amount { get; set; }
        public string note_to_player { get; set; }
        public PaypalSellerPayableBreakdown seller_payable_breakdown { get; set; }
    }
}

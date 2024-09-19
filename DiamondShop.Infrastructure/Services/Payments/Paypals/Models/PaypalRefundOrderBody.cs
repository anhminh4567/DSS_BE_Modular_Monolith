using DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals;
using Newtonsoft.Json;
using Syncfusion.XlsIO.FormatParser.FormatTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Paypals.Models
{
    public class PaypalRefundOrderBody
    {
        [JsonPropertyName("amount")]
        [JsonProperty("amount")]

        public PaypalAmount Amount { get;set; }
        [JsonPropertyName("note_to_payer")]
        [JsonProperty("note_to_payer")]

        public string? NoteToPayer { get; set; } = "refund the amount";
    }
}

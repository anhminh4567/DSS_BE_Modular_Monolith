using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Zalopays.Models.Responses
{
    public class ZalopayPaymentDataField
    {
        public int app_id { get; set; }
        public string app_trans_id { get; set; }
        public long app_time { get; set; }
        public string app_user {  get; set; }   
        public long amount { get; set; }
        [JsonProperty("embed_data")]
        public string embed_data_string { get; set; }
        public ZalopayEmbeddedData ZalopayEmbeddedData { get; set; }
        [JsonProperty("item")]
        public string item_string { get; set; }
        public List<ZalopayItem> Items { get; set; }
        public long zp_trans_id { get; set; }
        public long server_time { get; set; }
        public int channel { get; set; }
        public string merchant_user_id { get; set; }
        public long user_fee_amount { get; set; }
        public long discount_amount { get; set; }
    }
}

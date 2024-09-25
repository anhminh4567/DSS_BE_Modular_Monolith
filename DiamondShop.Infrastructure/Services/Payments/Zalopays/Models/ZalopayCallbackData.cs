using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Zalopays.Models
{
    public class ZalopayCallbackData
    {
        public int app_id { get; set; }

        public string app_trans_id { get; set; }

        public long app_time { get; set; }

        public string app_user { get; set; }

        public int amount { get; set; }

        public string embed_data { get; set; }

        public string item { get; set; }

        public long zp_trans_id { get; set; }

        public long server_time { get; set; }

        public int channel { get; set; }

        public string merchant_user_id { get; set; }

        public int user_fee_amount { get; set; }

        public int discount_amount { get; set; }
    }
}

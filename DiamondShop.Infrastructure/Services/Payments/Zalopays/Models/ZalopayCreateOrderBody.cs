using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Zalopays.Models
{
    public class ZalopayCreateOrderBody
    {
        //public int app_id { get; set; }  // app_id
        public string app_user { get; set; }  // app_user
        //public string app_trans_id { get; set; }  // app_trans_id
        //public long app_time { get; set; }  // app_time
        //public long expire_duration_seconds { get; set; }  // expire_duration_seconds
        public long amount { get; set; }  // amount
        public List<Item> item { get; set; }  // item (JSON Array String)
        public string? description { get; set; }  // description
        //public string embed_data { get; set; }  // embed_data (JSON String)
        public string bank_code { get; set; }  // bank_code
        //public string mac { get; set; }  // mac
        //public string callback_url { get; set; }  // callback_url
        //public string device_info { get; set; }  // device_info (JSON String)
        //public string sub_app_id { get; set; }  // sub_app_id
        public string title { get; set; }  // title
        //public string currency { get; set; } = "VND";// currency 
        public string? phone { get; set; }  // phone
        public string? email { get; set; }  // email
        public string? address { get; set; }  // address
    }
    public class Item
    {
        //this is the field of your app, not zalopay
        // Example:
        public string name { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal sale_price { get; set; }

    }
    public class ZalopayEmbeddedData
    {
        public string[] preferred_payment_method { get; set; } =  { };
        public string redirecturl { get; set; }
        //	JSON String	{"column_name": "value"}	
        //	Thêm thông tin hiển thị ở phần Quản lý giao dịch chi tiết trên Merchant site
        public string? columninfo { get; set; } 
    }
}

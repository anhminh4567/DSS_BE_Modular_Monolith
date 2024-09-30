using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Infrastructure.Services.Payments.Zalopays.Models;
using DiamondShop.Infrastructure.Services.Payments.Zalopays.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DiamondShop.Infrastructure.Services.Payments.Zalopays
{
    public class ZalopayClient
    {
        private static string appid = "2553";
        private static string key1 = "PcY4iZIKFCIdgZvA6ueMcMHHUbRLYjPL";
        private static string key2 = "eG4r0GcoNtRGbO8";

        private static string getBankListUrl = "https://sbgateway.zalopay.vn/api/getlistmerchantbanks";
        private static string create_order_url = "https://sb-openapi.zalopay.vn/v2/create";
        static string query_order_url = "https://sb-openapi.zalopay.vn/v2/query";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ZalopayClient> _logger;

        public ZalopayClient(IHttpContextAccessor httpContextAccessor, ILogger<ZalopayClient> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public static async Task<Dictionary<string, List<ZalopayBankDTO>>> GetBanks()
        {
            var reqtime = ZalopayUtils.GetTimeStamp().ToString();

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appid", appid);
            param.Add("reqtime", reqtime);
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, appid + "|" + reqtime));

            var result = await HttpHelper.PostFormAsync<ZalopayBankListResponse>(getBankListUrl, param);

            Console.WriteLine("returncode = {0}", result.returncode);
            Console.WriteLine("returnmessage = {0}", result.returnmessage);

            foreach (var entry in result.banks)
            {
                var pmcid = entry.Key;
                var banklist = entry.Value;
                foreach (var bank in banklist)
                {
                    Console.WriteLine("{0}. {1} - {2}", pmcid, bank.bankcode, bank.name);
                }
            }
            return result.banks;
        }
        public static async Task<Dictionary<string, object>> CreateOrder(ZalopayCreateOrderBody body, string redirectUrl, string callbackUrl)
        {
            Random rnd = new Random();
            //var embed_data = new { };
            //var items = new[] { new { } };
            var embed_data = new ZalopayEmbeddedData
            {
                columninfo = "",
                redirecturl = redirectUrl,
                preferred_payment_method = ["domestic_card", "account"],
            };
            var param = new Dictionary<string, string>();
            var app_trans_id = DateTime.UtcNow.ToString("yyyyMMddHHmmSS"); //rnd.Next(100000000); // Generate a random order's ID.

            param.Add("app_id", appid);
            param.Add("app_user", body.app_user);
            param.Add("app_time", ZalopayUtils.GetTimeStamp().ToString());
            param.Add("amount", "50000");
            param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
            param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
            param.Add("item", JsonConvert.SerializeObject(body.item));
            param.Add("description", "Lazada - Thanh toán đơn hàng #" + app_trans_id);
            param.Add("bank_code", "" );
            param.Add("callback_url", callbackUrl);
            var data = appid + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync(create_order_url, param);

            foreach (var entry in result)
            {
                Console.WriteLine("{0} = {1}", entry.Key, entry.Value);
            }
            return result;
        }
        public async Task<Dictionary<string, object>> Callback()
        {
            var httpClient = _httpContextAccessor.HttpContext;
            if (httpClient == null)
                throw new Exception("cannot found httpClient");
            var result = new Dictionary<string, object>();
            var cbData = await httpClient.Request.ReadFromJsonAsync<ZalopayCallbackRequest>();
            try
            {
                //    var dataStr = Convert.ToString(cbdata["data"]);
                //    var reqMac = Convert.ToString(cbdata["mac"]);
                var dataStr = Convert.ToString(cbData.data);
                var reqMac = Convert.ToString(cbData.mac);

                var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key2, dataStr);

                Console.WriteLine("mac = {0}", mac);

                // kiểm tra callback hợp lệ (đến từ ZaloPay server)
                if (!reqMac.Equals(mac))
                {
                    // callback không hợp lệ
                    result["return_code"] = -1;
                    result["return_message"] = "mac not equal";
                }
                else
                {
                    // thanh toán thành công
                    // merchant cập nhật trạng thái cho đơn hàng
                    var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                    Console.WriteLine("update order's status = success where app_trans_id = {0}", dataJson["app_trans_id"]);

                    result["return_code"] = 1;
                    result["return_message"] = "success";
                }
            }
            catch (Exception ex)
            {
                result["return_code"] = 0; // ZaloPay server sẽ callback lại (tối đa 3 lần)
                result["return_message"] = ex.Message;
            }

            // thông báo kết quả cho ZaloPay server
            return result;
        }
        public static async Task<Dictionary<string,object>> GetTransactionDetail(string app_transaction_id)
        {
           // var app_trans_id = "<app_trans_id>";  // Input your app_trans_id

            var param = new Dictionary<string, string>();
            param.Add("app_id", appid);
            param.Add("app_trans_id", app_transaction_id);
            var data = appid + "|" + app_transaction_id + "|" + key1;

            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync(query_order_url, param);

            foreach (var entry in result)
            {
                Console.WriteLine("{0} = {1}", entry.Key, entry.Value);
            }
            return result;
        }
    }
}


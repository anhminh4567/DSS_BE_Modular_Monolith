using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Infrastructure.Services.Payments.Zalopays.Models;
using DiamondShop.Infrastructure.Services.Payments.Zalopays.Models.Responses;
using DiamondShop.Infrastructure.Services.Payments.Zalopays.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DiamondShop.Infrastructure.Services.Payments.Zalopays
{
    public class ZalopayClient
    {
        private static string appid = "2554";
        private static string key1 = "sdngKKJmqEMzvh5QQcdD2A9XBSKUNaYn";
        private static string key2 = "trMrHtvjo6myautxDUiAcYsVtaeQ8nhf";

        private static string getBankListUrl = "https://sbgateway.zalopay.vn/api/getlistmerchantbanks";
        private static string create_order_url = "https://sb-openapi.zalopay.vn/v2/create";
        private static string query_order_url = "https://sb-openapi.zalopay.vn/v2/query";
        private static string refund_url = "https://sb-openapi.zalopay.vn/v2/refund";
        private static string query_refund_url = "https://sb-openapi.zalopay.vn/v2/query_refund";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ZalopayClient> _logger;

        public ZalopayClient(IHttpContextAccessor httpContextAccessor, ILogger<ZalopayClient> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public static async Task<Dictionary<string, List<ZalopayBankResponse>>> GetBanks()
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
        public async Task<ZalopayCreateOrderResponse> CreateOrder(ZalopayCreateOrderBody body, string redirectUrl, string callbackUrl)
        {
            Random rnd = new Random();
            //var embed_data = new { };
            //var items = new[] { new { } };
            var embed_data = new ZalopayEmbeddedData
            {
                columninfo = "",
                redirecturl = redirectUrl,
                preferred_payment_method = []//["domestic_card", "account"],
            };
            var param = new Dictionary<string, string>();
            var app_trans_id = DateTime.UtcNow.ToString("yyyyMMddHHmmss"); //rnd.Next(100000000); // Generate a random order's ID.

            param.Add("app_id", appid);
            param.Add("app_user", body.app_user);
            param.Add("app_time", ZalopayUtils.GetTimeStamp().ToString());
            param.Add("amount", body.amount.ToString());
            param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
            param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
            param.Add("item", JsonConvert.SerializeObject(body.item));
            param.Add("description", "Lazada - Thanh toán đơn hàng #" + app_trans_id);
            param.Add("bank_code", "");
            param.Add("callback_url", callbackUrl);
            var data = appid + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync<ZalopayCreateOrderResponse>(create_order_url, param);

            //foreach (var entry in result)
            //{
            //    Console.WriteLine("{0} = {1}", entry.Key, entry.Value);
            //}
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
                //var dataStr = Convert.ToString(cbdata["data"]);
                //var reqMac = Convert.ToString(cbdata["mac"]);
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
                    //var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                    var dataObject = JsonConvert.DeserializeObject<ZalopayPaymentDataField>(dataStr);
                    var embedDataObject = JsonConvert.DeserializeObject<ZalopayEmbeddedData>(dataObject.embed_data_string);
                    var itemObjectList = JsonConvert.DeserializeObject<List<ZalopayItem>>(dataObject.item_string);
                    dataObject.ZalopayEmbeddedData = embedDataObject;
                    dataObject.Items = itemObjectList;
                    Console.WriteLine("update order's status = success where app_trans_id = {0}", dataObject.app_trans_id);
                    //if () { } // check neu thanh cong, check DB xem transaction ton tai hay chuaw thi tra ve return_code = 1
                    //if () { } // neu ton tai Transaction, laf transaction da callback roi => tra ve = 2, la giao dich da xu ly
                    //if () { } // con lai thi loi, ko callback


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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app_transaction_id">
        /// the id that the app give to zalopay, generated from app
        /// </param>
        /// <returns></returns>
        public async Task<ZalopayTransactionResponse> GetTransactionDetail(string app_transaction_id)
        {
            // var app_trans_id = "<app_trans_id>";  // Input your app_trans_id

            var param = new Dictionary<string, string>();
            param.Add("app_id", appid);
            param.Add("app_trans_id", app_transaction_id);
            var data = appid + "|" + app_transaction_id + "|" + key1;

            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync< ZalopayTransactionResponse>(query_order_url, param);

            //foreach (var entry in result)
            //{
            //    Console.WriteLine("{0} = {1}", entry.Key, entry.Value);
            //}
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zalo_trans_id">
        /// zalo return an id when finish transaction, save this for refunding
        /// </param>
        /// <param name="amount">
        /// amount to refund, the checking should be done in the application layer
        /// </param>
        /// <param name="refund_Fee_From_Merchant">
        /// amount that you will subtract from payment of customer for your platform, leave empty if refund full
        /// </param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ZalopayRefundResponse> RefundTransaction(string zalo_trans_id, long amount, long refund_Fee_From_Merchant = 0, string description = "NONE")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(zalo_trans_id);
            ArgumentNullException.ThrowIfNull(amount);
            if (amount <= 0)
                throw new Exception("invalid amount to refund");
            var timestamp = ZalopayUtils.GetTimeStamp().ToString();
            var rand = new Random();
            var uid = timestamp + "" + rand.Next(111, 999).ToString();

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("app_id", appid);
            param.Add("m_refund_id", DateTime.UtcNow.ToString("yyMMdd") + "_" + appid + "_" + uid);
            param.Add("zp_trans_id", zalo_trans_id);
            param.Add("amount", amount.ToString());
            param.Add("timestamp", timestamp);
            param.Add("description", description);
            string data = null;
            if (refund_Fee_From_Merchant > 0)
            {
                if (refund_Fee_From_Merchant < amount)
                {
                    param.Add("refund_fee_amount", refund_Fee_From_Merchant.ToString());
                    data = appid + "|" + param["zp_trans_id"] + "|" + param["amount"] + "|" + param["refund_fee_amount"] + "|" + param["description"] + "|" + param["timestamp"];
                }
                else
                    throw new Exception("refund fee is greater than the total amount of refund");
            }
            else
            {
                data = appid + "|" + param["zp_trans_id"] + "|" + param["amount"] + "|" + param["description"] + "|" + param["timestamp"];
            }
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync<ZalopayRefundResponse>(refund_url, param);

            //foreach (var entry in result)
            //{
            //    Console.WriteLine("{0} = {1}", entry.Key, entry.Value);
            //}
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="merchant_refund_id">
        /// Mã giao dịch merchant tự gen đã truyền qua lúc gọi hoàn tiền
        /// • Định dạng: yymmdd_appid_xxxxxxxxxx
        /// </param>
        /// <param name="timeStamp">
        /// Thời điểm gọi api (timestamp in milisecond)
        /// </param>
        /// <returns></returns>
        public async Task<ZalopayRefundResponse> GetRefundTransactionDetail(string merchant_refund_id,string timeStamp)
        {

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("app_id", appid);
            param.Add("timestamp", timeStamp);
            param.Add("m_refund_id", merchant_refund_id);//"190308_2553_xxxxxx");

            var data = appid + "|" + param["m_refund_id"] + "|" + param["timestamp"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync<ZalopayRefundResponse>(query_refund_url, param);

            //foreach (var entry in result)
            //{
            //    Console.WriteLine("{0} = {1}", entry.Key, entry.Value);
            //}
            return result;
        }
    }
}


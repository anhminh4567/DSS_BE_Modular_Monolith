using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Vnpayment;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Vnpays
{
    internal class VnpayQuery
    {
        private readonly IOptions<VnpayOption> _options;

        public VnpayQuery(IOptions<VnpayOption> options)
        {
            _options = options;
        }

        public VnpayQueryResult? Query(string txtRef, long transactionDate)
        {
            var vnpayOpt = _options.Value;
            var vnp_Api = vnpayOpt.Vnp_Query_Url;
            var vnp_HashSecret = vnpayOpt.Vnp_HashSecret; //Secret KEy
            var vnp_TmnCode = vnpayOpt.Vnp_TmnCode; // Terminal Id

            var vnp_RequestId = DateTime.UtcNow.Ticks.ToString(); //Mã hệ thống merchant tự sinh ứng với mỗi yêu cầu truy vấn giao dịch. Mã này là duy nhất dùng để phân biệt các yêu cầu truy vấn giao dịch. Không được trùng lặp trong ngày.
            var vnp_Version = vnpayOpt.Version; //2.1.0
            var vnp_Command = "querydr";
            var vnp_TxnRef = txtRef; //orderId.Text; // Mã giao dịch thanh toán tham chiếu
            var vnp_OrderInfo = "Truy van giao dich:" + txtRef;//orderId.Text;
            var vnp_TransactionDate = transactionDate;//payDate.Text;
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnp_IpAddr = "0.0.0.0";

            var signData = vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TxnRef + "|" + vnp_TransactionDate + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo;
            var vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);

            var qdrData = new
            {
                vnp_RequestId,
                vnp_Version,
                vnp_Command,
                vnp_TmnCode,
                vnp_TxnRef,
                vnp_OrderInfo,
                vnp_TransactionDate,
                vnp_CreateDate,
                vnp_IpAddr,
                vnp_SecureHash

            };
            var jsonData = JsonConvert.SerializeObject(qdrData);//.Serialize(qdrData);

            using (var vnpayClient = new HttpClient() { BaseAddress = new Uri(vnp_Api), })
            {
                //vnpayClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");//(jsonData);
                var response = vnpayClient.PostAsync(new Uri(vnp_Api), content).Result;
                var contentBody = response.Content.ReadFromJsonAsync<VnpayQueryResult>().Result;
                return contentBody;
            }
        }
    }
}

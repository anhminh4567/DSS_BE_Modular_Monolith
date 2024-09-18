using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Vnpayment;
using DiamondShop.Infrastructure.Services.Payments.Vnpays.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Vnpays
{
    public record VnpayRefundCommand(string vnpay_TxnRef, string vnp_PayDate, VnpayRefundType refundType, Money refundedAmount, string note = null);

    public class VnpayRefund
    {
        private readonly IOptions<VnpayOption> _options;
        private readonly IHttpContextAccessor _contextAccessor;

        public VnpayRefund(IOptions<VnpayOption> options, IHttpContextAccessor contextAccessor)
        {
            _options = options;
            _contextAccessor = contextAccessor;
        }
        public VnpayRefundResult? Execute(VnpayRefundCommand vnpayRefundCommand)
        {
            VnpayOption vnpayOption = _options.Value;
            HttpContext httpContext = _contextAccessor.HttpContext is null 
                ? throw new ArgumentNullException() 
                : _contextAccessor.HttpContext;

            var vnp_Api = vnpayOption.Vnp_Url;
            var vnp_HashSecret = vnpayOption.Vnp_HashSecret;
            var vnp_TmnCode = vnpayOption.Vnp_TmnCode;// Terminal Id

            var vnp_RequestId = DateTime.UtcNow.Ticks.ToString(); //Mã hệ thống merchant tự sinh ứng với mỗi yêu cầu hoàn tiền giao dịch. Mã này là duy nhất dùng để phân biệt các yêu cầu truy vấn giao dịch. Không được trùng lặp trong ngày.
            var vnp_Version = vnpayOption.Version; //2.1.0
            var vnp_Command = "refund";
            var vnp_TransactionType = vnpayRefundCommand.refundType;
            var vnp_Amount = Convert.ToInt64(vnpayRefundCommand.refundedAmount.Value) * 100;
            var vnp_TxnRef = vnpayRefundCommand.vnpay_TxnRef; // Mã giao dịch thanh toán tham chiếu
            var vnp_OrderInfo = "Hoan tien giao dich:" + vnpayRefundCommand.vnpay_TxnRef;
            var vnp_TransactionNo = ""; //Giả sử giá trị của vnp_TransactionNo không được ghi nhận tại hệ thống của merchant.
            var vnp_TransactionDate = vnpayRefundCommand.vnp_PayDate;
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnp_CreateBy = "diamond shop";
            var vnp_IpAddr = Utils.GetIpAddress(httpContext);

            var signData = vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TransactionType + "|" + vnp_TxnRef + "|" + vnp_Amount + "|" + vnp_TransactionNo + "|" + vnp_TransactionDate + "|" + vnp_CreateBy + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo;
            var vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);

            var rfData = new
            {
                vnp_RequestId = vnp_RequestId,
                vnp_Version = vnp_Version,
                vnp_Command = vnp_Command,
                vnp_TmnCode = vnp_TmnCode,
                vnp_TransactionType = vnp_TransactionType,
                vnp_TxnRef = vnp_TxnRef,
                vnp_Amount = vnp_Amount,
                vnp_OrderInfo = vnp_OrderInfo,
                vnp_TransactionNo = vnp_TransactionNo,
                vnp_TransactionDate = vnp_TransactionDate,
                vnp_CreateBy = vnp_CreateBy,
                vnp_CreateDate = vnp_CreateDate,
                vnp_IpAddr = vnp_IpAddr,
                vnp_SecureHash = vnp_SecureHash

            };
            var jsonData = JsonConvert.SerializeObject(rfData);

            using (RestClient client = new RestClient(new Uri(vnpayOption.Vnp_Api)))
            {
                string nullstring= null;
                RestRequest request = new RestRequest(nullstring, method: Method.Post);
                request.AddStringBody(jsonData, ContentType.Json);
                request.AddHeader("Content-Type", "application/json; charset=utf-8");
                client.AddDefaultHeader("Accept","application/json");
                var response = client.Execute<VnpayRefundResult>(request);
                return response.Data;
            }
        }
        public void HandleRefundResponse(VnpayRefundResult result)
        {
            
        }
    }
}

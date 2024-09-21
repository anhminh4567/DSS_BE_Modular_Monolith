using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Vnpayment;
using DiamondShop.Infrastructure.Services.Payments.Vnpays.Constants;
using DiamondShop.Infrastructure.Services.Payments.Vnpays.Models;
using FluentResults;
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
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Vnpays
{
    public record VnpayRefundCommand(string vnpay_TxnRef, string vnp_PayDate, string refundType, string amountInVIETNAMDONG, string note = null);

    public class VnpayRefund
    {
        private readonly IOptions<VnpayOption> _options;
        private readonly IHttpContextAccessor _contextAccessor;
        private const string NullString = null;
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

            var vnp_Api = vnpayOption.Vnp_Api;
            var vnp_HashSecret = vnpayOption.Vnp_HashSecret;
            var vnp_TmnCode = vnpayOption.Vnp_TmnCode;// Terminal Id

            var vnp_RequestId = DateTime.Now.Ticks.ToString(); //Mã hệ thống merchant tự sinh ứng với mỗi yêu cầu hoàn tiền giao dịch. Mã này là duy nhất dùng để phân biệt các yêu cầu truy vấn giao dịch. Không được trùng lặp trong ngày.
            var vnp_Version = vnpayOption.Version; //2.1.0
            var vnp_Command = "refund";
            if (vnpayRefundCommand.refundType.Equals(VnpayRefundType.HoanTraToanPhan) is false &&
                vnpayRefundCommand.refundType.Equals(VnpayRefundType.HoanTraMotPhan) is false)
            {
                throw new Exception($"unknonw refund type, can only be {VnpayRefundType.HoanTraMotPhan}  OR {VnpayRefundType.HoanTraToanPhan}");
            }
            var vnp_TransactionType = vnpayRefundCommand.refundType.ToString();
            var vnp_Amount = Convert.ToInt64(vnpayRefundCommand.amountInVIETNAMDONG) * 100;
            var vnp_TxnRef = vnpayRefundCommand.vnpay_TxnRef; // Mã giao dịch thanh toán tham chiếu
            var vnp_OrderInfo = "Hoan tien giao dich:" + vnpayRefundCommand.vnpay_TxnRef;
            var vnp_TransactionNo = "14585533"; //Giả sử giá trị của vnp_TransactionNo không được ghi nhận tại hệ thống của merchant.
            var vnp_TransactionDate = vnpayRefundCommand.vnp_PayDate;
            var vnp_CreateDate = DateTime.Now.ToString(vnpayOption.Vnp_DateTime_Format);
            var vnp_CreateBy = "diamondshop";
            var vnp_IpAddr = Utils.GetIpAddress(httpContext);

            var signData = vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TransactionType + "|" + vnp_TxnRef + "|" + vnp_Amount + "|" + vnp_TransactionNo + "|" + vnp_TransactionDate + "|" + vnp_CreateBy + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo;
            var vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);

            var rfData = new
            {
                vnp_RequestId,
                vnp_Version,
                vnp_Command,
                vnp_TmnCode,
                vnp_TransactionType,
                vnp_TxnRef,
                vnp_Amount,
                vnp_OrderInfo,
                vnp_TransactionNo,
                vnp_TransactionDate,
                vnp_CreateBy,
                vnp_CreateDate,
                vnp_IpAddr,
                vnp_SecureHash

            };
            var jsonData = JsonConvert.SerializeObject(rfData);

            using (var vnpayClient = new HttpClient() { BaseAddress = new Uri(vnp_Api), })
            {
                //vnpayClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");//(jsonData);
                var response = vnpayClient.PostAsync(NullString, content).Result;
                var contentBody = response.Content.ReadFromJsonAsync<VnpayRefundResult>().Result;
                return contentBody;
            }

        }
        public async Task<Result<VnpayRefundResult>> MOCK_VnpayRefund(VnpRefundData vnpRefundData)
        {
            var transactionId = vnpRefundData.vnp_TxnRef;
            var createDate = vnpRefundData.vnp_CreateDate;
            var refundType = vnpRefundData.vnp_TransactionType;
            var amount = vnpRefundData.vnp_Amount;

            if(refundType.Equals(VnpayTransactionType.HOAN_TIEN_FULL))
            {

            }
            if (refundType.Equals(VnpayTransactionType.HOAN_TIEN_1_PHAN))
            {

            }

            return Result.Ok();
        }
    }
}

using Azure.Core;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Vnpayment;
using DiamondShop.Infrastructure.Services.Payments.Vnpays.Models;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Vnpays
{
    public class VnpayReturn
    {
        private readonly IOptions<VnpayOption> _options;
        private readonly IHttpContextAccessor _contextAccessor;

        public VnpayReturn(IOptions<VnpayOption> options, IHttpContextAccessor contextAccessor)
        {
            _options = options;
            _contextAccessor = contextAccessor;
        }
        public Result<VnpayReturnResult> Execute()
        {
            VnpayOption vnpayOption = _options.Value;
            HttpContext httpContext = _contextAccessor.HttpContext is null
                ? throw new ArgumentNullException()
                : _contextAccessor.HttpContext;
            HttpRequest request = httpContext.Request;

            if (request.QueryString.Value.Length > 0)
            {
                string vnp_HashSecret = vnpayOption.Vnp_HashSecret;
                var vnpayData = request.Query;
                VnpayLibrary vnpay = new VnpayLibrary();

                foreach (string s in vnpayData.Keys)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                string vnp_SecureHash = request.Query["vnp_SecureHash"];
                string TerminalID = request.Query["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                string bankCode = request.Query["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

                var returnResult = new VnpayReturnResult()
                {
                    Amount = vnp_Amount,
                    BankCode = bankCode,
                    StatusCode = vnp_ResponseCode,
                    TransactionNo = vnpayTranId,
                    TransactionStatus = vnp_TransactionStatus,
                    Vnp_TxnRef = orderId,
                };
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                        return Result.Ok(returnResult);
                    else
                        return Result.Fail("Có lỗi xảy ra trong quá trình xử lý.Mã lỗi:" + vnp_ResponseCode);
                }
                else
                {
                    return Result.Fail("Có lỗi xảy ra trong quá trình xử lý.Mã lỗi:" + vnp_ResponseCode);
                }
            }
            throw new Exception("no parameter found");
        }
    }
}
//log.InfoFormat("Invalid signature, InputData={0}", Request.RawUrl);
//displayMsg.InnerText = "Có lỗi xảy ra trong quá trình xử lý";

//Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
//displayMsg.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
//log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId,vnpayTranId, vnp_ResponseCode);

////Thanh toan thanh cong
//displayMsg.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
//    log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId,vnpayTranId);

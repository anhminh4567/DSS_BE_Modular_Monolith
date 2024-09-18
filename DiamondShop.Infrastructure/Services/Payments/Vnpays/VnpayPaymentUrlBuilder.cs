using Azure;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Vnpayment;
using DiamondShop.Infrastructure.Services.Payments.Vnpays.Models;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Vnpays
{
    public class VnpayPaymentUrlBuilder
    {
        private readonly VnpayOption _vnpayOption;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<VnpayPaymentUrlBuilder> _logger;
        public VnpayPaymentUrlBuilder(IOptions<VnpayOption> options, IHttpContextAccessor httpContextAccessor, ILogger<VnpayPaymentUrlBuilder> logger)
        {
            _vnpayOption = options.Value;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public Result<string> GetPaymentUrl()
        {
            string vnp_Returnurl = _vnpayOption.Vnp_Return_Url; //URL nhan ket qua tra ve 
            string vnp_Url = _vnpayOption.Vnp_Url; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _vnpayOption.Vnp_TmnCode; //Ma website
            string vnp_HashSecret = _vnpayOption.Vnp_HashSecret; //Chuoi bi mat
            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            {
                return Result.Fail("Vui lòng cấu hình các tham số: vnp_TmnCode,vnp_HashSecret trong file web.config");
            }
            //Get payment input
            VnpayOrderInfo order = new VnpayOrderInfo();
            //Save order to db
            order.OrderId = DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            order.Amount = 100000; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending"
            order.OrderDesc = "not important";
            order.CreatedDate = DateTime.Now;
            //string locale = cboLanguage.SelectedItem.Value;
            //Build URL for VNPAY
            VnpayLibrary vnpay = new VnpayLibrary();

            vnpay.AddRequestData("vnp_Version", VnpayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString());
            //Số tiền thanh toán. Số tiền không 

            //mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND

            //(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY

            //là: 10000000


            //if (cboBankCode.SelectedItem != null && !string.IsNullOrEmpty(cboBankCode.SelectedItem.Value))
            //{
            //    vnpay.AddRequestData("vnp_BankCode", cboBankCode.SelectedItem.Value);
            //}

            string createDate = order.CreatedDate.ToString(_vnpayOption.Vnp_DateTime_Format);//this thing will be used for refund/query/anything
            vnpay.AddRequestData("vnp_CreateDate", createDate);
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_httpContextAccessor.HttpContext));

            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId + " | tai thoi diem: " +createDate);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);

            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ 
            //thống của merchant.Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY.Không được
            //trùng lặp trong ngày
            vnpay.AddRequestData("vnp_ExpireDate", order.CreatedDate.AddMinutes(_vnpayOption.Vnp_Payment_Timeout_Minute).ToString(_vnpayOption.Vnp_DateTime_Format));
            //Billing


            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //_logger.LogInformation("VNPAY URL: {paymentUrl}", paymentUrl);
            //Response.Redirect(paymentUrl);
            return Result.Ok(paymentUrl);
        }
    }
}

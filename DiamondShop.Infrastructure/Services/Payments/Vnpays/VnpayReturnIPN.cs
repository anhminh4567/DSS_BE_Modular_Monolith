using Azure.Core;
using Azure;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Vnpayment;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Infrastructure.Services.Payments.Vnpays.Models;

namespace DiamondShop.Infrastructure.Services.Payments.Vnpays
{
    public class VnpayReturnIPN
    {
        private readonly IOptions<VnpayOption> _options;
        private readonly IHttpContextAccessor _contextAccessor;

        public VnpayReturnIPN(IOptions<VnpayOption> options, IHttpContextAccessor contextAccessor)
        {
            _options = options;
            _contextAccessor = contextAccessor;
        }
        public async Task Execute()
        {
            VnpayOption vnpayOption = _options.Value;
            HttpContext httpContext = _contextAccessor.HttpContext is null
                ? throw new ArgumentNullException()
                : _contextAccessor.HttpContext;
            HttpRequest request = httpContext.Request;

            string returnContent = string.Empty;
            if (request.Query.Count > 0)
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
                //Lay danh sach tham so tra ve tu VNPAY
                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                string vnp_SecureHash = request.Query["vnp_SecureHash"];
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    //Cap nhat ket qua GD
                    //Yeu cau: Truy van vao CSDL cua  Merchant => lay ra duoc OrderInfo
                    //Giả sử OrderInfo lấy ra được như giả lập bên dưới
                    VnpayOrderInfo order = new VnpayOrderInfo();//get from DB
                    order.OrderId = orderId;
                    order.Amount = 100000;
                    order.PaymentTranId = vnpayTranId;
                    order.Status = "0"; //0: Cho thanh toan,
                                        //1: da thanh toan,
                                        //2: GD loi
                    //Kiem tra tinh trang Order
                    if (order != null)
                    {
                        if (order.Amount == vnp_Amount)
                        {
                            if (order.Status == "0")
                            {
                                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                                {
                                    //Thanh toan thanh cong
                                    //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId,
                                    //    vnpayTranId);
                                    order.Status = "1";
                                }
                                else
                                {
                                    //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                                    //  displayMsg.InnerText = "Có lỗi xảy ra trong quá trình xử lý. 
                                    //Mã lỗi: " + vnp_ResponseCode;
                                    //    log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}",
                                    //        orderId, vnpayTranId, vnp_ResponseCode);
                                    order.Status = "2";
                                }

                                //Thêm code Thực hiện cập nhật vào Database 
                                //Update Database
                                returnContent = "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
                            }
                            else
                            {
                                returnContent = "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
                            }
                        }
                        else
                        {
                            returnContent = "{\"RspCode\":\"04\",\"Message\":\"invalid amount\"}";
                        }
                    }
                    else
                    {
                        returnContent = "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
                    }
                }
                else
                {
                    //log.InfoFormat("Invalid signature, InputData={0}", Request.RawUrl);
                    returnContent = "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}";
                }
            }
            else
            {
                returnContent = "{\"RspCode\":\"99\",\"Message\":\"Input data required\"}";
            }


            httpContext.Response.Clear();
            await httpContext.Response.WriteAsync(returnContent);
        }
    }
}

using DiamondShop.Application.Dtos.Responses.Transactions;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Vnpayment;
using DiamondShop.Infrastructure.Services.Payments.Vnpays.Models;
using DiamondShop.Infrastructure.Services.Payments.Zalopays;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static Syncfusion.XlsIO.Parser.Biff_Records.ExternSheetRecord;

namespace DiamondShop.Infrastructure.Services.Payments.Vnpays
{
    internal class VnpayPaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IMemoryCache _cache;
        private readonly IOptions<UrlOptions> _urlOptions;
        private readonly IOptions<VnpayOption> _vnpayOptions;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly ILogger<VnpayPaymentService> _logger;
        private readonly IDbCachingService _dbCachingService;
        private readonly ISender _sender;
        private readonly IPublisher _publisher;

        public VnpayPaymentService(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, ITransactionRepository transactionRepository, IPaymentMethodRepository paymentMethodRepository, IHttpContextAccessor httpContextAccessor, IOrderTransactionService orderTransactionService, IMemoryCache cache, IOptions<UrlOptions> urlOptions, IOptions<VnpayOption> options, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, ILogger<VnpayPaymentService> logger, IDbCachingService dbCachingService, ISender sender, IPublisher publisher)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _transactionRepository = transactionRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _httpContextAccessor = httpContextAccessor;
            _orderTransactionService = orderTransactionService;
            _cache = cache;
            _urlOptions = urlOptions;
            _vnpayOptions = options;
            _optionsMonitor = optionsMonitor;
            _logger = logger;
            _dbCachingService = dbCachingService;
            _sender = sender;
            _publisher = publisher;
        }

        public async Task<object> Callback()
        {

            VnpayOption vnpayOption = _vnpayOptions.Value;
            HttpContext httpContext = _httpContextAccessor.HttpContext is null? throw new ArgumentNullException(): _httpContextAccessor.HttpContext;
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

            return  returnContent;
            //httpContext.Response.Clear();
            //await httpContext.Response.WriteAsync(returnContent);
        }

        public async Task<Result<PaymentLinkResponse>> CreatePaymentLink(PaymentLinkRequest paymentLinkRequest, CancellationToken cancellationToken = default)
        {
            string vnp_Returnurl = _vnpayOptions.Value.Vnp_Return_Url;//_vnpayOption.Vnp_Return_Url; //URL nhan ket qua tra ve 
            string vnp_Url = _vnpayOptions.Value.Vnp_Url; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _vnpayOptions.Value.Vnp_TmnCode; //Ma website
            string vnp_HashSecret = _vnpayOptions.Value.Vnp_HashSecret; //Chuoi bi mat
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


            string createDate = order.CreatedDate.ToString(_vnpayOptions.Value.Vnp_DateTime_Format);//this thing will be used for refund/query/anything
            vnpay.AddRequestData("vnp_CreateDate", createDate);
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_httpContextAccessor.HttpContext));

            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId + " | tai thoi diem: " + createDate);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);

            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ 
            //thống của merchant.Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY.Không được
            //trùng lặp trong ngày
            vnpay.AddRequestData("vnp_ExpireDate", order.CreatedDate.AddMinutes(_vnpayOptions.Value.Vnp_Payment_Timeout_Minute).ToString(_vnpayOptions.Value.Vnp_DateTime_Format));
            //Billing


            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Result.Ok(new PaymentLinkResponse { PaymentUrl = paymentUrl });
            throw new NotImplementedException();
        }

        public Task<PaymentRefundDetail> GetRefundDetail(Transaction refundTransactionType)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentDetail> GetTransactionDetail(Transaction payTrasactionType)
        {
            var vnpayOpt = _vnpayOptions.Value;
            var vnp_Api = vnpayOpt.Vnp_Api;
            var vnp_HashSecret = vnpayOpt.Vnp_HashSecret; //Secret KEy
            var vnp_TmnCode = vnpayOpt.Vnp_TmnCode; // Terminal Id

            var vnp_RequestId = DateTime.UtcNow.Ticks.ToString(); //Mã hệ thống merchant tự sinh ứng với mỗi yêu cầu truy vấn giao dịch. Mã này là duy nhất dùng để phân biệt các yêu cầu truy vấn giao dịch. Không được trùng lặp trong ngày.
            var vnp_Version = vnpayOpt.Version; //2.1.0
            var vnp_Command = "querydr";
            var vnp_TxnRef = payTrasactionType.AppTransactionCode; //txtRef; //orderId.Text; // Mã giao dịch thanh toán tham chiếu
            var vnp_OrderInfo = "Truy van giao dich:" + payTrasactionType.AppTransactionCode;//txtRef;//orderId.Text;
            var vnp_TransactionDate = payTrasactionType.TimeStamp; ;//payDate.Text;
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
                return new PaymentDetail()
                {
                    AppReceiveAmount = long.Parse( contentBody.vnp_Amount),
                       PaygateTransactionId = contentBody.vnp_TransactionNo,
                       ReturnCode = int.Parse(contentBody.vnp_ResponseCode),
                       ReturnMessage = contentBody.vnp_TransactionStatus,
                       
                };
                //return contentBody;
            }
            throw new NotImplementedException();
        }

        public Task<Result<PaymentRefundDetail>> Refund(Order order, Transaction forTransaction, decimal fineAmount, string description = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllPaymentCache(Order order)
        {
            throw new NotImplementedException();
        }
    }
}

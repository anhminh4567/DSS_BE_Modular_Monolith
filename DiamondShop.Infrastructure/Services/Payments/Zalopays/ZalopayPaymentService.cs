using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Infrastructure.Services.Payments.Zalopays.Models.Responses;
using DiamondShop.Infrastructure.Services.Payments.Zalopays.Models;
using DiamondShop.Infrastructure.Services.Payments.Zalopays.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Orders.ValueObjects;

namespace DiamondShop.Infrastructure.Services.Payments.Zalopays
{
    internal class ZalopayPaymentService : IPaymentService
    {
        private readonly ZalopayClient _zalopayClient;
        private readonly ILogger<ZalopayPaymentService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderTransactionService _orderTransactionService;
        private static string appid = "2554";
        private static string key1 = "sdngKKJmqEMzvh5QQcdD2A9XBSKUNaYn";
        private static string key2 = "trMrHtvjo6myautxDUiAcYsVtaeQ8nhf";

        private static string getBankListUrl = "https://sbgateway.zalopay.vn/api/getlistmerchantbanks";
        private static string create_order_url = "https://sb-openapi.zalopay.vn/v2/create";
        private static string query_order_url = "https://sb-openapi.zalopay.vn/v2/query";
        private static string refund_url = "https://sb-openapi.zalopay.vn/v2/refund";
        private static string query_refund_url = "https://sb-openapi.zalopay.vn/v2/query_refund";

        public ZalopayPaymentService(ZalopayClient zalopayClient, ILogger<ZalopayPaymentService> logger, IUnitOfWork unitOfWork, IOrderRepository orderRepository, ITransactionRepository transactionRepository, IPaymentMethodRepository paymentMethodRepository, IHttpContextAccessor httpContextAccessor, IOrderTransactionService orderTransactionService)
        {
            _zalopayClient = zalopayClient;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _transactionRepository = transactionRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _httpContextAccessor = httpContextAccessor;
            _orderTransactionService = orderTransactionService;
        }

        public async Task<object> Callback()
        {
            var httpClient = _httpContextAccessor.HttpContext;
            if (httpClient == null)
                throw new Exception("cannot found httpClient");
            var result = new Dictionary<string, object>();
            var cbData = await httpClient.Request.ReadFromJsonAsync<ZalopayCallbackRequest>();
            try
            {
                var dataStr = Convert.ToString(cbData.data);
                var reqMac = Convert.ToString(cbData.mac);
                var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key2, dataStr);
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
                    var dataObject = JsonConvert.DeserializeObject<ZalopayPaymentDataField>(dataStr);
                    var embedDataObject = JsonConvert.DeserializeObject<ZalopayEmbeddedData>(dataObject.embed_data_string);
                    var itemObjectList = JsonConvert.DeserializeObject<List<ZalopayItem>>(dataObject.item_string);
                    PaymentMetadataBodyPerTransaction metaData = JsonConvert.DeserializeObject<PaymentMetadataBodyPerTransaction>(embedDataObject.columninfo);
                    dataObject.ZalopayEmbeddedData = embedDataObject;
                    dataObject.Items = itemObjectList;
                    var appGivenId = dataObject.app_trans_id;
                    var paymentGateId = dataObject.zp_trans_id.ToString();
                    ArgumentNullException.ThrowIfNull(appGivenId);
                    ArgumentNullException.ThrowIfNull(paymentGateId);
                    // ta thu lay transaction, neu da ton tai thi check status roi tinh tiep
                    // neu chua ton tai ==> auto save transaction
                    Transaction? tryGetTransaction = await _transactionRepository.GetByAppAndPaygateId(appGivenId,paymentGateId);
                    List<PaymentMethod> paymentMethods = await _paymentMethodRepository.GetAll();
                    var zalopayMethod = paymentMethods.First(x => x.MethodName.ToUpper() == "ZALOPAY");
                    _logger.LogInformation("update order's status = success where app_trans_id = {0}", dataObject.app_trans_id);
                    if (tryGetTransaction == null) // check neu thanh cong, check DB xem transaction ton tai hay chuaw thi tra ve return_code = 1
                    {
                        var orderIdParsed = OrderId.Parse(metaData.ForOrderId);
                        var newTran = Transaction.CreatePayment(zalopayMethod.Id, orderIdParsed, metaData.Description,dataObject.app_trans_id,dataObject.zp_trans_id.ToString(),metaData.TimeStampe,dataObject.amount,DateTime.UtcNow);
                        await _transactionRepository.Create(newTran);
                        result["return_code"] = 1;
                        result["return_message"] = "success";
                    }
                    else// neu ton tai Transaction, laf transaction da callback roi => tra ve = 2, la giao dich da xu ly
                    {
                        result["return_code"] = 2;
                        result["return_message"] = "success";
                    } 
                    //if () { } // con lai thi loi, ko callback
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

        public async Task<Result<PaymentLinkResponse>> CreatePaymentLink(PaymentLinkRequest paymentLinkRequest, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(paymentLinkRequest.Amount);
            var embed_data = new ZalopayEmbeddedData
            {
                columninfo = "",
                redirecturl = paymentLinkRequest.ReturnUrl ,
                preferred_payment_method = ["domestic_card", "account"],
            };
            var param = new Dictionary<string, string>();
            var app_trans_id = DateTime.UtcNow.ToString("yyyyMMddHHmmss"); //rnd.Next(100000000); // Generate a random order's ID.
            var userId = paymentLinkRequest.Account.Id.Value;
            var amount = paymentLinkRequest.Amount;
            var timeStampe = ZalopayUtils.GetTimeStamp().ToString();
            var description = paymentLinkRequest.Description is null ? $"thanh toan cho don hang {paymentLinkRequest.Order.Id.Value}, timestampe = {timeStampe}" : paymentLinkRequest.Description;
            PaymentMetadataBodyPerTransaction descriptionBodyJson = new PaymentMetadataBodyPerTransaction
            {
                GeneratedCode = app_trans_id,
                TimeStampe = timeStampe,
                ForAccountId = paymentLinkRequest.Account.Id.Value,
                ForOrderId = paymentLinkRequest.Order.Id.Value,
                Description = description,
            };
            //insert meta data
            embed_data.columninfo = JsonConvert.SerializeObject(descriptionBodyJson);

            param.Add("app_id", appid);
            param.Add("app_user", userId);
            param.Add("app_time", timeStampe);
            param.Add("amount", amount.ToString());
            param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
            param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
            //param.Add("item", JsonConvert.SerializeObject(body.item));
            param.Add("description", description);
            param.Add("bank_code", "");
            param.Add("callback_url", paymentLinkRequest.CallbackUrl);
            var data = appid + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));
            var result = await HttpHelper.PostFormAsync<ZalopayCreateOrderResponse>(create_order_url, param);
            if (result.return_code != 1)
                return Result.Fail($"fail with message from paygate: {result.sub_return_message} and code: {result.return_code} ");
            return Result.Ok(new PaymentLinkResponse { PaymentUrl = result.order_url ,QrCode = result.qr_code });
        }

        public Task GetRefundDetail(Transaction refundTransactionType)
        {
            throw new NotImplementedException();
        }

        public Task GetTransactionDetail(Transaction payTrasactionType)
        {
            throw new NotImplementedException();
        }

        public Task<Result> Refund(Order order)
        {
            throw new NotImplementedException();
        }

        public Task GetTransactionDetail(System.Transactions.Transaction payTrasactionType)
        {
            throw new NotImplementedException();
        }

        public Task GetRefundDetail(System.Transactions.Transaction refundTransactionType)
        {
            throw new NotImplementedException();
        }
    }
}

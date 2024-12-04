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
using DiamondShop.Infrastructure.Services.Payments.Zalopays.Constants;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.BusinessRules;
using OpenQA.Selenium.DevTools.V127.Page;
using Microsoft.Extensions.Options;
using DiamondShop.Infrastructure.Options;
using MediatR;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Common;
using Microsoft.Extensions.Caching.Memory;
using DiamondShop.Domain.Models.Transactions.Events;
using Azure.Storage.Blobs.Models;
using QRCoder;

namespace DiamondShop.Infrastructure.Services.Payments.Zalopays
{
    internal class ZalopayPaymentService : IPaymentService
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
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly ILogger<ZalopayPaymentService> _logger;
        private readonly IDbCachingService _dbCachingService;
        private readonly ISender _sender;
        private readonly IPublisher _publisher;
        private static string appid = "2554";
        private static string key1 = "sdngKKJmqEMzvh5QQcdD2A9XBSKUNaYn";
        private static string key2 = "trMrHtvjo6myautxDUiAcYsVtaeQ8nhf";
        private static string ReturnUri = "api/Zalopay/Return";
        private static string CallbackUri = "api/Zalopay/Callback";


        private static string getBankListUrl = "https://sbgateway.zalopay.vn/api/getlistmerchantbanks";
        private static string create_order_url = "https://sb-openapi.zalopay.vn/v2/create";
        private static string query_order_url = "https://sb-openapi.zalopay.vn/v2/query";
        private static string refund_url = "https://sb-openapi.zalopay.vn/v2/refund";
        private static string query_refund_url = "https://sb-openapi.zalopay.vn/v2/query_refund";

        public ZalopayPaymentService(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, ITransactionRepository transactionRepository, IPaymentMethodRepository paymentMethodRepository, IHttpContextAccessor httpContextAccessor, IOrderTransactionService orderTransactionService, IMemoryCache cache, IOptions<UrlOptions> urlOptions, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, ILogger<ZalopayPaymentService> logger, IDbCachingService dbCachingService, ISender sender, IPublisher publisher)
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
            _optionsMonitor = optionsMonitor;
            _logger = logger;
            _dbCachingService = dbCachingService;
            _sender = sender;
            _publisher = publisher;
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
                    Transaction? tryGetTransaction = await _transactionRepository.GetByAppAndPaygateId(appGivenId, paymentGateId);
                    List<PaymentMethod> paymentMethods = await _paymentMethodRepository.GetAll();
                    var orderIdParsed = OrderId.Parse(metaData.ForOrderId);
                    var getOrderDetail = await _orderRepository.GetById(orderIdParsed);
                    var zalopayMethod = paymentMethods.First(x => x.MethodName.ToUpper() == PaymentMethod.ZALOPAY.MethodName.ToUpper());
                    _logger.LogInformation("update order's status = success where app_trans_id = {0}", dataObject.app_trans_id);
                    if (tryGetTransaction == null || getOrderDetail.Status == OrderStatus.Cancelled || getOrderDetail.Status == OrderStatus.Rejected) // check neu thanh cong, check DB xem transaction ton tai hay chuaw thi tra ve return_code = 1
                    {
                        await _unitOfWork.BeginTransactionAsync();
                        var newTran = Transaction.CreatePayment(zalopayMethod.Id, orderIdParsed, metaData.Description, dataObject.app_trans_id, dataObject.zp_trans_id.ToString(), metaData.TimeStampe, dataObject.amount, DateTime.UtcNow);
                        newTran.VerifyZalopay(dataObject.zp_trans_id.ToString(), metaData.TimeStampe);
                        await _transactionRepository.Create(newTran);
                        result["return_code"] = 1;
                        result["return_message"] = "success";
                        //getOrderDetail.Status = OrderStatus.Processing;
                        //getOrderDetail.PaymentStatus = getOrderDetail.PaymentType == PaymentType.Payall ? PaymentStatus.PaidAll : PaymentStatus.Deposited;
                        await _publisher.Publish(new TransactionCreatedEvent(newTran, DateTime.UtcNow));
                        await _orderRepository.Update(getOrderDetail);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();

                    }
                    else// neu ton tai Transaction, laf transaction da callback roi => tra ve = 2, la giao dich da xu ly
                    {
                        if (tryGetTransaction != null)
                        {
                            if (tryGetTransaction.Status == TransactionStatus.Verifying)
                            {
                                tryGetTransaction.VerifyZalopay(dataObject.zp_trans_id.ToString(), metaData.TimeStampe);
                                result["return_code"] = 1;
                                result["return_message"] = "success";
                                //getOrderDetail.Status = OrderStatus.Processing;
                                //getOrderDetail.PaymentStatus = getOrderDetail.PaymentType == PaymentType.Payall ? PaymentStatus.PaidAll : PaymentStatus.Deposited;
                                await _publisher.Publish(new TransactionCreatedEvent(tryGetTransaction, DateTime.UtcNow));
                                await _orderRepository.Update(getOrderDetail);
                                await _unitOfWork.SaveChangesAsync();
                                await _unitOfWork.CommitAsync();
                            }
                        }
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

        public async Task<Result<PaymentLinkResponse>> CreatePaymentLink(PaymentLinkRequest paymentLinkRequest, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(paymentLinkRequest.Amount);
            ArgumentNullException.ThrowIfNull(paymentLinkRequest.Order);
            var paymentMethods = await _paymentMethodRepository.GetAll();
            var zalopayMethod = paymentMethods.First(x => x.Id == PaymentMethod.ZALOPAY.Id);
            var order = paymentLinkRequest.Order;
            var orderTransaction = await _transactionRepository.GetByOrderId(order.Id);
            var payTransaction = orderTransaction.Where(t => t.TransactionType == TransactionType.Pay).OrderByDescending(x => x.InitDate).ToList();
            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Rejected)
            {
                return Result.Fail("order not valid to create payment");
            }
            if (order.Status == OrderStatus.Pending)
            {
                if (order.ExpiredDate != null)
                {
                    var expriredDate = order.ExpiredDate.Value;
                    if (expriredDate <= DateTime.UtcNow)
                        return Result.Fail("order expired already");
                }
            }
            if (order.PaymentStatus != PaymentStatus.Pending && order.PaymentStatus != PaymentStatus.Deposited)
            {
                return Result.Fail("payment status is not in pending or deposit state, only these 2 are allowed to create payment");
            }
            if (order.Status == OrderStatus.Prepared && order.FinishPreparedDate == null)
            {
                return Result.Fail("order is not finish prepared yet to create payment link, wait for complete preparation");
            }
            if (order.Status == OrderStatus.Prepared && order.FinishPreparedDate != null)
            {
                if (order.IsCollectAtShop == false)
                {
                    return Result.Fail("order is not valid state to get transaction link");
                }
            }
            var transactionOption = _optionsMonitor.CurrentValue.TransactionRule;
            if (paymentLinkRequest.Amount > transactionOption.MaximumPerTransaction)
            {
                return Result.Fail($"amount is greater than {transactionOption.MaximumPerTransaction}");
            }
            Transaction? zalopayTransactionOnGoing = payTransaction.Where(t => t.Status == TransactionStatus.Verifying).FirstOrDefault();
            var cacheKey = GetCacheKey(paymentLinkRequest.Order);
            if (zalopayTransactionOnGoing != null)
            {
                return new PaymentLinkResponse() { PaymentUrl = zalopayTransactionOnGoing.GetPaymentLink(), QrCode = GenQRImagePng(zalopayTransactionOnGoing.GetPaymentLink()) };
            }
            //string? paymentLink;
            //var tryGetFromCache = await _dbCachingService.Get(cacheKey);// _cache.Get(cacheKey);
            //if (tryGetFromCache != null)
            //{
            //    var parseResult = tryGetFromCache.GetObjectFromJsonString<ZalopayCreateOrderResponse>() ;
            //    paymentLink = parseResult.order_url;
            //    return new PaymentLinkResponse() { PaymentUrl = paymentLink, QrCode = GenQRImagePng(paymentLink) };
            //}
            double? secondsExpired = GetSecondPaymentTimeOut(paymentLinkRequest.Order);
            if (secondsExpired == null)
            {
                return Result.Fail("order is not in pending or delivering state");
            }
            var callbackUrl = string.Concat(_urlOptions.Value.HttpsUrl, "/", CallbackUri);
            var returnUrl = string.Concat(_urlOptions.Value.HttpsUrl, "/", ReturnUri);
            //var returnUrl = string.Concat("https://3wkrskcn-7160.asse.devtunnels.ms", "/", ReturnUri);
            var embed_data = new ZalopayEmbeddedData
            {
                columninfo = "",
                redirecturl = returnUrl,
                preferred_payment_method = []//["domestic_card", "account"],
            };
            var correctAmount = paymentLinkRequest.Amount; //_orderTransactionService.GetCorrectAmountFromOrder(order);

            var param = new Dictionary<string, string>();
            var app_trans_id = order.PaymentStatus == PaymentStatus.Pending
                ? order.CreatedDate.ToString("yyyyMMddHHmmss")
                : order.FinishPreparedDate.Value.ToString("yyyyMMddHHmmss");
            var userId = paymentLinkRequest.Account.Id.Value;
            //var amount = paymentLinkRequest.Amount;
            var amount = correctAmount;
            var timeStampe = order.PaymentStatus == PaymentStatus.Pending
                ? ZalopayUtils.GetTimeStamp(DateTime.Now).ToString()
                : ZalopayUtils.GetTimeStamp(DateTime.Now).ToString();
            var description = paymentLinkRequest.Description is null ? $"thanh toan cho don hang {paymentLinkRequest.Order.OrderCode}, timestampe = {timeStampe}" : paymentLinkRequest.Description;
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
            var appTransactionId = order.CreatedDate.ToLocalTime().ToString("yyMMdd") + "_" + app_trans_id;
            List<ZalopayItem> falseList = new List<ZalopayItem>() { new ZalopayItem() { name = "order", price = amount, quantity = 1, sale_price = amount } };
            param.Add("app_id", appid);
            param.Add("app_user", userId);
            param.Add("app_time", timeStampe);
            param.Add("amount", amount.ToString());
            param.Add("app_trans_id", appTransactionId); // mã giao dich có định dạng yyMMdd_xxxx //DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id
            param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
            param.Add("item", JsonConvert.SerializeObject(falseList));
            param.Add("description", description);
            param.Add("bank_code", "");
            param.Add("expire_duration_seconds", ((int)secondsExpired).ToString()); // 15 minutes (900 seconds)
            param.Add("callback_url", callbackUrl);
            var data = appid + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));
            var result = await HttpHelper.PostFormAsync<ZalopayCreateOrderResponse>(create_order_url, param);
            if (result.return_code != 1)
                return Result.Fail($"fail with message from paygate: {result.sub_return_message} and code: {result.return_code} ");

            var temporalTransaction = Transaction.CreatePayment(zalopayMethod.Id, order.Id, description, appTransactionId, result.order_url, timeStampe, amount, DateTime.UtcNow);
            temporalTransaction.Status = TransactionStatus.Verifying;
            await _transactionRepository.Create(temporalTransaction);
            await _unitOfWork.SaveChangesAsync();
            //await _dbCachingService.SetValue(new DbCacheModel()
            //{
            //    CreationTime= DateTime.UtcNow,
            //    KeyId = cacheKey,
            //    Name = cacheKey,
            //    Value = JsonConvert.SerializeObject(result),
            //    Type = typeof(ZalopayCreateOrderResponse).GetType().AssemblyQualifiedName
            //});
            return Result.Ok(new PaymentLinkResponse { PaymentUrl = result.order_url, QrCode = GenQRImagePng(result.order_url) });
        }

        public async Task<PaymentRefundDetail> GetRefundDetail(Transaction refundTransactionType)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("app_id", appid);
            param.Add("timestamp", refundTransactionType.TimeStamp);
            param.Add("m_refund_id", refundTransactionType.AppTransactionCode);//"190308_2553_xxxxxx");

            var data = appid + "|" + param["m_refund_id"] + "|" + param["timestamp"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync<ZalopayRefundResponse>(query_refund_url, param);
            var code = result.return_code.ToString();
            bool isProcessing = code switch
            {
                ZalopayReturnCode.PROCESSING => true,
                _ => false
            };
            return new PaymentRefundDetail()
            {
                ReturnCode = result.return_code,
                SubReturnMessage = result.sub_return_message,
                SubReturnCode = result.sub_return_code,
                ReturnMessage = result.return_message,
                IsProcessing = isProcessing,
                FineAmount = refundTransactionType.FineAmount,
                RefundAmount = refundTransactionType.TransactionAmount,
                PaygateTransactionId = refundTransactionType.PaygateTransactionCode
            };
        }

        public async Task<Result<PaymentRefundDetail>> Refund(Order order, Transaction forTransaction, decimal fineAmount, string description = null)
        {
            var getTransactions = await _transactionRepository.GetByOrderId(order.Id);
            if ((order.TotalRefund + order.TotalFine) >= order.TotalPrice)
            {
                return Result.Fail("order is fully refunded");
            }
            var totalPaidTransaction = getTransactions.Where(t => t.TransactionType == TransactionType.Pay).Sum(t => t.TransactionAmount);
            var totalRefundTransaction = getTransactions.Where(t => t.TransactionType == TransactionType.Refund).Sum(t => t.TotalAmount);

            var timestamp = ZalopayUtils.GetTimeStamp().ToString();
            var rand = new Random();
            var uid = timestamp + "" + rand.Next(111, 999).ToString();
            var appMerchantGenId = DateTime.UtcNow.ToString("yyMMdd") + "_" + appid + "_" + uid;
            var fineAmountRounded = MoneyVndRoundUpRules.RoundAmountFromDecimal(fineAmount);
            //var refundAmount = (long) forTransaction.TotalAmount - fineAmountRounded; 
            //if (refundAmount <= 0)
            //    throw new Exception("invalid refund amount");

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("app_id", appid);
            param.Add("m_refund_id", appMerchantGenId);
            param.Add("zp_trans_id", forTransaction.PaygateTransactionCode);
            param.Add("amount", ((long)forTransaction.TotalAmount).ToString());
            param.Add("timestamp", timestamp);
            param.Add("description", description is null ? $"hoan tra don hang cho giao dich: {forTransaction.AppTransactionCode} cua don hang: {order.OrderCode} " : description);
            string data = null;
            if (fineAmountRounded > 0)
            {
                if (fineAmountRounded < forTransaction.TotalAmount)
                {
                    param.Add("refund_fee_amount", fineAmountRounded.ToString());
                    data = appid + "|" + param["zp_trans_id"] + "|" + param["amount"] + "|" + param["refund_fee_amount"] + "|" + param["description"] + "|" + param["timestamp"];
                }
                else
                    throw new Exception("refund fee is greater than the total amount of refund");
            }
            else
                data = appid + "|" + param["zp_trans_id"] + "|" + param["amount"] + "|" + param["description"] + "|" + param["timestamp"];

            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync<ZalopayRefundResponse>(refund_url, param);
            if (result.return_code.ToString() == ZalopayReturnCode.FAIL)
            {
                return Result.Fail($"fail with message from paygate: {result.sub_return_message} and code: {result.return_code} ");
            }
            PaymentRefundDetail paymentRefundDetail = null;
            var transaction = Transaction.CreateRefund(forTransaction.PayMethodId, order.Id, forTransaction.Id, description, appMerchantGenId, result.refund_id.ToString(), timestamp, forTransaction.TotalAmount, fineAmountRounded);
            if (result.return_code.ToString() == ZalopayReturnCode.PROCESSING)
            {
                _logger.Log(LogLevel.Information, "refund is processing, need to check later");
                // try to get 3 times 
                for (int i = 0; i < 5; i++)
                {
                    paymentRefundDetail = await GetRefundDetail(transaction);
                    if (paymentRefundDetail.IsProcessing)
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                    else
                    {
                        if (paymentRefundDetail.ReturnCode.ToString() == ZalopayReturnCode.FAIL)
                            return Result.Fail($"fail with message from paygate: {paymentRefundDetail.SubReturnMessage} and code: {paymentRefundDetail.ReturnCode} ");
                        else //if you success
                            break;
                    }
                }
            }
            ArgumentNullException.ThrowIfNull(paymentRefundDetail);
            await _unitOfWork.BeginTransactionAsync();
            await _transactionRepository.Create(transaction);
            order.AddRefund(transaction);
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return Result.Ok(paymentRefundDetail);
        }

        public async Task<PaymentDetail> GetTransactionDetail(Transaction payTrasactionType)
        {
            var param = new Dictionary<string, string>();
            param.Add("app_id", appid);
            param.Add("app_trans_id", payTrasactionType.AppTransactionCode);
            var data = appid + "|" + payTrasactionType.AppTransactionCode + "|" + key1;
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));
            var result = await HttpHelper.PostFormAsync<ZalopayTransactionResponse>(query_order_url, param);
            return new PaymentDetail()
            {
                AppReceiveAmount = result.amount,
                IsProcessing = result.is_processing,
                PaygateTransactionId = result.zp_trans_id.ToString(),
                ReturnCode = result.return_code,
                ReturnMessage = result.return_message,
                SubReturnCode = result.sub_return_code,
                SubReturnMessage = result.sub_return_message
            };
        }
        private string GetCacheKey(Order order)
        {
            var statusString = order.PaymentStatus.ToString();
            return $"ZP_{order.Id.Value}_{statusString}";
        }
        private double? GetSecondPaymentTimeOut(Order order)
        {
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Delivering)
            {
                if (order.IsCollectAtShop == false)
                    return null;
            }
            //if(order.ExpiredDate != null)
            //    return null;
            if (order.PaymentStatus == PaymentStatus.Paid)
                return null;
            //if (order.Status == OrderStatus.Pending)
            if (order.PaymentStatus == PaymentStatus.Pending)
            {
                var orderEndDateExpected = order.ExpiredDate.Value;//order.CreatedDate.AddHours(OrderRules.ExpiredOrderHour).ToUniversalTime();
                double trueTimeRemainForPaymentLink = (orderEndDateExpected - DateTime.UtcNow).TotalSeconds;
                trueTimeRemainForPaymentLink = Math.Floor(trueTimeRemainForPaymentLink);
                return trueTimeRemainForPaymentLink;
            }
            //else if (order.Status == OrderStatus.Delivering)
            else if (order.PaymentStatus == PaymentStatus.Deposited)
            {
                if (order.FinishPreparedDate == null)
                    return null;
                var orderRule = _optionsMonitor.CurrentValue.OrderRule;
                var correctExpiredSeconds = order.FinishPreparedDate.Value.AddDays(orderRule.DaysWaitForCustomerToPay).ToUniversalTime();
                TimeSpan span = correctExpiredSeconds - DateTime.UtcNow;
                if (span <= TimeSpan.Zero)
                    return null;
                double trueTimeRemainForPaymentLink = Math.Floor(span.TotalSeconds);
                return trueTimeRemainForPaymentLink;
            }
            else
                return null;
        }
        private string GenQRImagePng(string paymentUrl)
        {
            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(paymentUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(QrCodeInfo);
            var qrCodeImage = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
        }

        public async Task RemoveAllPaymentCache(Order order)
        {
            var getCacheKeyBase = $"ZP_{order.Id.Value}";
            _dbCachingService.RemoveValues(getCacheKeyBase).Wait();
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using Microsoft.Extensions.Options;
using Net.payOS.Types;
using Net.payOS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DiamondShop.Infrastructure.Services
{
    internal class ExternalBankService : IExternalBankServices
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderService _orderService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private const string CLIENT_ID = "bd3a5fa5-8918-44bc-b59d-bd6a1d72fd28";
        private const string CHECKSUM_KEY = "3e28c31a5ca6e1ea35bfe4d5be6fb6a3c3e4497bb9c3d5e279cbde8639b8d6a4"; //c9a184be283e934e896ed0977e86232426f7611d469e755e96b38e3de1e43e87
        private const string API_KEY = "98c73257-0f6d-41a6-8151-88d93f5a5912";
        public ExternalBankService(IOrderRepository orderRepository, IOrderTransactionService orderTransactionService, IOrderService orderService, ITransactionRepository transactionRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _orderRepository = orderRepository;
            _orderTransactionService = orderTransactionService;
            _orderService = orderService;
            _transactionRepository = transactionRepository;
            _optionsMonitor = optionsMonitor;
        }

        public ExternalBankQrcodeDto GenerateQrCodeFromOrder(Order order, decimal amount)
        {
            string url = "https://api.vietqr.io/v2/generate";
            var shopBank = _optionsMonitor.CurrentValue.ShopBankAccountRules;
//            var correctAmount = _orderTransactionService.GetCorrectAmountFromOrder(order);
            var correctAmount  = amount;

            var genRequestDto = new VietQrGenRequetDto(long.Parse(shopBank.AccountNumber),shopBank.AccountName,int.Parse(shopBank.BankBin), correctAmount,$"#{order.OrderCode}","text", "print");;
            using HttpClient client = new HttpClient();
            string jsonBody = JsonConvert.SerializeObject(genRequestDto);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Send the POST request
            HttpResponseMessage response =  client.PostAsync(url, content).Result;

            // Ensure a successful response
            response.EnsureSuccessStatusCode();
            VietQrGenResponse parseResult = response.Content.ReadFromJsonAsync<VietQrGenResponse>().Result;
            var qrCode = parseResult.data.qrCode;
            var qrDataUrl = parseResult.data.qrDataURL;
            var accName = parseResult.data.accountName;

            return new ExternalBankQrcodeDto(qrCode, qrDataUrl);
            throw new NotImplementedException();
        }

        public ExternalBankTransactionDetailDto GetTransactionDetail(Order order, Domain.Models.Transactions.Transaction transaction)
        {
            var orderCode = transaction.PaygateTransactionCode;
            PayOS payOS = new PayOS(CLIENT_ID, API_KEY, CHECKSUM_KEY);
            PaymentLinkInformation paymentLinkInfomation =  payOS.getPaymentLinkInformation(long.Parse(orderCode)).Result;
        https://api-merchant.payos.vn/v2/payment-requests
            throw new NotImplementedException();
        }
    }
    public record VietQrGenRequetDto(
        long accountNo,
        string accountName,
        int acqId,
        decimal amount,
        string addInfo,
        string format,
        string template);
    public record VietQrGenResponse(
        string code,
        string desc,
        VietQrData? data);

    public record VietQrData(
        int acpId,
        string accountName,
        string qrCode,
        string qrDataURL
    );
}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DiamondShop.Domain.Models.AccountAggregate;

namespace DiamondShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ApiControllerBase
    {
        private readonly IExternalBankServices _externalBankServices;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public TestController(IExternalBankServices externalBankServices, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _externalBankServices = externalBankServices;
            _optionsMonitor = optionsMonitor;
        }
        [HttpGet]
        public async Task<ActionResult> TestPaymentQrFromPayosVietQr()
        {
            var account = Account.Create(FullName.Create("a", "b"), "testingwebandstuff@gmail.com");
            var order = Order.Create(account.Id, PaymentType.Payall, PaymentMethodId.Parse("1"), 50_000_000, 20_000, 0, "abc", null, null, 40_000, 20_000, DateTime.UtcNow, OrderId.Parse("1"));
            order.Items.Add(OrderItem.Create(order.Id, null, DiamondId.Parse("1"), 25_000_000, 20_000_000, null, null, null, null, 0));
            order.Items.Add(OrderItem.Create(order.Id, JewelryId.Parse("2"), null, 25_000_000, 20_000_000, null, null, null, null, 0));
            return Ok( _externalBankServices.GenerateQrCodeFromOrder(order,50_000_000));
        }
    }
}

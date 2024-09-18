using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Paypals.Models;
using DiamondShop.Infrastructure.Services.Payments.Paypals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DiamondShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaypalController : ControllerBase
    {
        private readonly IOptions<PaypalOption> _paypal;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaypalController(IOptions<PaypalOption> paypal, IHttpContextAccessor httpContextAccessor)
        {
            _paypal = paypal;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("/paypalresponse")]
        [HttpGet]
        public async Task<ActionResult> paypal()
        {
            var paypalClient = new PaypalClient(_paypal);
            return Ok(await paypalClient.GetAccessToken());
        }
        [Route("/paypalCreateOrder")]
        [HttpPost]
        public async Task<ActionResult> paypalCreateOrder(PaypalCreateOrderBody paypalCreateOrderBody)
        {
            var paypalClient = new PaypalClient(_paypal);
            var result = await paypalClient.CreateOrder(paypalCreateOrderBody);
            return Ok(result);
        }
        [Route("/paypalOrderId")]
        [HttpGet]
        public async Task<ActionResult> paypalOrderDetail([FromQuery] string paypalOrderId)
        {
            var paypalClient = new PaypalClient(_paypal);
            var result = await paypalClient.ShowOrderDetail(paypalOrderId);
            return Ok(result);
        }
        [Route("/paypalCaptureOrder")]
        [HttpGet]
        public async Task<ActionResult> paypalCaptureOrder([FromQuery] string paypalOrderId)
        {
            var paypalClient = new PaypalClient(_paypal);
            var result = await paypalClient.CaptureOrder(paypalOrderId);
            return Ok(result);
        }
    }
}

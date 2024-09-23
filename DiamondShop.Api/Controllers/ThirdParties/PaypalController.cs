using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Paypals.Models;
using DiamondShop.Infrastructure.Services.Payments.Paypals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DiamondShop.Api.Controllers.ThirdParties
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaypalController : ApiControllerBase
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
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [Route("/paypalOrderId")]
        [HttpGet]
        public async Task<ActionResult> paypalOrderDetail([FromQuery] string paypalOrderId)
        {
            var paypalClient = new PaypalClient(_paypal);
            var result = await paypalClient.ShowOrderDetail(paypalOrderId);
            return Ok(result);
        }
        [Route("/paypalCapturePayment")]
        [HttpGet]
        public async Task<ActionResult> paypalCapturePayment([FromQuery] string paypalOrderId)
        {
            var paypalClient = new PaypalClient(_paypal);
            var result = await paypalClient.CaptureOrder(paypalOrderId);
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [Route("/paypalRefundTransaction")]
        [HttpPost]
        public async Task<ActionResult> paypalRefundTransaction([FromQuery] string paypalTransactionId, [FromBody] PaypalRefundOrderBody paypalRefundOrderBody)
        {
            var paypalClient = new PaypalClient(_paypal);
            var result = await paypalClient.RefundTransaction(paypalTransactionId, paypalRefundOrderBody);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return MatchError(result.Errors, ModelState);
        }
        [Route("/paypalShowPaymentDetail")]
        [HttpGet]
        public async Task<ActionResult> paypalPaymentDetail([FromQuery] string paypalOrderId)
        {
            var paypalClient = new PaypalClient(_paypal);
            var result = await paypalClient.ShowCapturedPaymentDetail(paypalOrderId);
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [Route("/paypalShowRefundDetail")]
        [HttpGet]
        public async Task<ActionResult> paypalRefundDetail([FromQuery] string refundTransactionId)
        {
            var paypalClient = new PaypalClient(_paypal);
            var result = await paypalClient.ShowRefundDetail(refundTransactionId);
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
    }
}


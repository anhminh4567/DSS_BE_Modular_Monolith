﻿using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Vnpays;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DiamondShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnpayController : ApiControllerBase
    {
        private readonly IOptions<VnpayOption> _vnpayOption;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VnpayController(IOptions<VnpayOption> vnpayOption, IHttpContextAccessor httpContextAccessor)
        {
            _vnpayOption = vnpayOption;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("/VnpayBuildUrl")]
        [HttpGet]
        public async Task<ActionResult> VnpayBuildUrl()
        {
            var vnpayClient = new VnpayPaymentUrlBuilder(_vnpayOption, _httpContextAccessor, null);
            //paypalClient.GetPaymentUrl();
            return Ok(vnpayClient.GetPaymentUrl().Value);
        }
        [Route("/VnpayReturnUrl")]
        [HttpGet]
        public async Task<ActionResult> VnpayReturn()
        {
            var vnpayClient = new VnpayReturn(_vnpayOption, _httpContextAccessor);
            var result = vnpayClient.Execute();
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [Route("/VnpayReturnIPN")]
        [HttpGet]
        public async Task<ActionResult> VnpayReturnIPN()
        {
            var vnpayClient = new VnpayReturnIPN(_vnpayOption, _httpContextAccessor);
            var result = vnpayClient.Execute();
            return Ok();
        }
        [Route("/VnpayQuery")]
        [HttpGet]
        public async Task<ActionResult> VnpayQuery(string vnp_txnRef, long time)
        {
            var vnpayClient = new VnpayQuery(_vnpayOption);
            var result = vnpayClient.Query(vnp_txnRef, time);
            return Ok(result);
        }
        [Route("/VnpayRefund")]
        [HttpPost]
        public async Task<ActionResult> VnpayRefund(VnpayRefundCommand vnpayRefundCommand)
        {
            var vnpayClient = new VnpayRefund(_vnpayOption, _httpContextAccessor);
            var result = vnpayClient.Execute(vnpayRefundCommand);
            return Ok(result);
        }
    }
}

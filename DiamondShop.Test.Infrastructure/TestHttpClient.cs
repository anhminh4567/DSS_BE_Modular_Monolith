using DiamondShop.Infrastructure.Services.Payments.Vietqr;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.Infrastructure
{
	public class TestHttpClient
	{
        [Fact]
        public async Task TestHttpClientVietQr()
        {
            VietqrPaymentService.GetToken().Wait();
        }
        [Fact]
        public async Task TestCreatePaymentLink()
        {
            VietqrPaymentService.GetToken().Wait();
        }

    }
}

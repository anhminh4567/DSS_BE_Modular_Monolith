using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Paypals.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals;
using Microsoft.AspNetCore.Http;
using FluentResults;
using System.Net.Http;
using DiamondShop.Infrastructure.Services.Payments.Paypals.Constants;
using DiamondShop.Domain.Common.ValueObjects;
namespace DiamondShop.Infrastructure.Services.Payments.Paypals
{
    public class PaypalClient
    {
        private readonly IOptions<PaypalOption> _options;

        public PaypalClient(IOptions<PaypalOption> options)
        {
            _options = options;
        }
        
        public async Task<Result<PaypalCreateOrderResult>> CreateOrder(PaypalCreateOrderBody paypalCreateOrderBody)
        {
            PaypalOption paypalOption = _options.Value;
            string accessToken = await GetAccessToken();
            string creatOrderUri = "/v2/checkout/orders";

            paypalCreateOrderBody.intent = PaypalIntent.CAPTURE;
            var content = new StringContent(JsonConvert.SerializeObject(paypalCreateOrderBody), null, "application/json");
            var content2 = JsonContent.Create(paypalCreateOrderBody);//(JsonConvert.SerializeObject(paypalCreateOrderBody), null, "application/json");
            using (var httpClient = new HttpClient()) 
            {
                //httpClient.BaseAddress = new Uri(creatOrderUri + creatOrderUri);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(paypalOption.Url+ creatOrderUri) );
                requestMessage.Headers.Add( "Authorization","Bearer " + accessToken);
                requestMessage.Content = content;
                var result = await httpClient.SendAsync(requestMessage);
                if(result.IsSuccessStatusCode) 
                {
                    var value = await result.Content.ReadFromJsonAsync<PaypalCreateOrderResult>();
                    var status = value.Status;
                    if(status != PaypalOrderStatus.CREATED) 
                    {
                        return Result.Fail("paypal order is created but the status is : " + status);
                    }
                    return value;
                }
                var value2 = await result.Content.ReadAsStringAsync();

                throw new Exception("result return with code: " + result.StatusCode);
            }
        }
        public async Task<PaypalOrderDetail?> ShowOrderDetail(string paypalGivenId) 
        {
            ArgumentNullException.ThrowIfNull(paypalGivenId);
            PaypalOption paypalOption = _options.Value;
            string accessToken = await GetAccessToken();
            string orderDetailUri = "/v2/checkout/orders/"+paypalGivenId;
            using(var httpClient = new HttpClient()) 
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(paypalOption.Url + orderDetailUri));
                requestMessage.Headers.Add("Authorization", "Bearer " + accessToken);
                var result = await httpClient.SendAsync(requestMessage);
                if (result.IsSuccessStatusCode) 
                {
                    var value = await result.Content.ReadFromJsonAsync<PaypalOrderDetail>();
                    return value;
                }
                var message = result.Content.ReadAsStringAsync().Result;
                return null;
            }

        }

        public async Task<Result<PaypalOrderDetail>> CaptureOrder(string paypalGivenId)
        {
            ArgumentNullException.ThrowIfNull(paypalGivenId);
            PaypalOption paypalOption = _options.Value;
            string accessToken = await GetAccessToken();
            string captureOrderUri = "/v2/checkout/orders/" + paypalGivenId + "/capture";
            using (var httpClient = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(paypalOption.Url + captureOrderUri));
                requestMessage.Headers.Add("Authorization", "Bearer " + accessToken);
                requestMessage.Content = new StringContent("",null,"application/json");
                var result = await httpClient.SendAsync(requestMessage);
                if (result.IsSuccessStatusCode)
                {
                    var value = await result.Content.ReadFromJsonAsync<PaypalOrderDetail>();
                    return value;
                }
                var message = result.Content.ReadAsStringAsync().Result;
                return Result.Fail(message);
            }
        }
        public async Task<Result<PaypalRefundDetail>> RefundTransaction(string paypalGivenId, PaypalRefundOrderBody paypalRefundOrderBody)
        {
            ArgumentNullException.ThrowIfNull(paypalGivenId);
            PaypalOption paypalOption = _options.Value;
            if (paypalRefundOrderBody.Amount.currency_code != Money.USD)
                return Result.Fail("curreny is forced to be : "+ Money.USD+ "| this is paypal purpose");
            string accessToken = await GetAccessToken();
            string captureOrderUri = "/v2/payments/captures/" + paypalGivenId + "/refund";
            using (var httpsClient = new HttpClient()) 
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(paypalOption.Url + captureOrderUri));
                httpRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                httpRequest.Content = new StringContent(JsonConvert.SerializeObject(paypalRefundOrderBody),null, "application/json");
                var result = await httpsClient.SendAsync(httpRequest);
                if (result.IsSuccessStatusCode)
                {
                    return Result.Ok(await result.Content.ReadFromJsonAsync<PaypalRefundDetail>());
                }
                return Result.Fail($"fail with status code: {result.StatusCode} ,and have body: {await result.Content.ReadAsStringAsync()}");
            }
        }
        public async Task<Result<PaypalRefundDetail>> ShowRefundDetail(string paypalRefundId)
        {
            PaypalOption paypalOption = _options.Value;
            string accessToken = await GetAccessToken();
            string paymentDetailURL = "/v2/payments/refunds/" + paypalRefundId;
            using (var httpClient = new HttpClient())
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, paypalOption.Url + paymentDetailURL);
                httpRequest.Content = new StringContent("", null, "application/json");
                httpRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                var result = await httpClient.SendAsync(httpRequest);
                if (result.IsSuccessStatusCode)
                {
                    return Result.Ok(await result.Content.ReadFromJsonAsync<PaypalRefundDetail>());
                }
                return Result.Fail($"fail with status code: {result.StatusCode} ,and have body: {await result.Content.ReadAsStringAsync()}");
            }
        }
        public async Task<Result<PaypalPaymentDetail>> ShowCapturedPaymentDetail(string paypalPaymentId)
        {
            PaypalOption paypalOption = _options.Value;
            string accessToken = await GetAccessToken();
            string paymentDetailURL = "/v2/payments/captures/" + paypalPaymentId;
            using(var httpClient = new HttpClient()) {
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, paypalOption.Url +  paymentDetailURL);
                httpRequest.Content = new StringContent("", null, "application/json");
                httpRequest.Headers.Add("Authorization","Bearer "+accessToken);
                var result = await httpClient.SendAsync(httpRequest);
                if (result.IsSuccessStatusCode)
                {
                    return Result.Ok(await result.Content.ReadFromJsonAsync<PaypalPaymentDetail>());
                }
                return Result.Fail($"fail with status code: {result.StatusCode}  and have body: {await result.Content.ReadAsStringAsync()}");
            }
        }
        public async Task<string> GetAccessToken()
        {
            PaypalOption paypalOpt = _options.Value;
            string oauthPath = "/v1/oauth2/token";
            //header value
            byte[] headerBytes = Encoding.UTF8.GetBytes($"{paypalOpt.ClientId}:{paypalOpt.ClientSecret}");
            string base64ClientIdClientSecret = Convert.ToBase64String(headerBytes);
            //body value
            var content = new StringContent("grant_type=client_credentials",null, "application/x-www-form-urlencoded");
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(paypalOpt.Url + oauthPath);
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + base64ClientIdClientSecret);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                var requestMessage = new HttpRequestMessage(HttpMethod.Post,paypalOpt.Url + oauthPath);
                requestMessage.Content = content;
                var httpResult = await client.SendAsync(requestMessage);
                if (httpResult.IsSuccessStatusCode is false)
                {
                    throw new Exception("why this is not success");
                }
                PaypalTokenResponse? paypalTokenResponse = await httpResult.Content.ReadFromJsonAsync<PaypalTokenResponse>();
                ArgumentNullException.ThrowIfNull(paypalTokenResponse);
                return paypalTokenResponse.access_token;
            }
        }
    }
}

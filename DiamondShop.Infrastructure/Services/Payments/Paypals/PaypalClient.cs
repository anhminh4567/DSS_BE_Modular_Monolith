using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Paypals.Models;
using FluentResults;
using Microsoft.Extensions.Options;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using DiamondShop.Infrastructure.Services.Payments.Paypals.Models.Generals;
namespace DiamondShop.Infrastructure.Services.Payments.Paypals
{
    public class PaypalClient
    {
        private readonly IOptions<PaypalOption> _options;

        public PaypalClient(IOptions<PaypalOption> options)
        {
            _options = options;
        }
        
        public async Task<PaypalCreateOrderResult> CreateOrder(PaypalCreateOrderBody paypalCreateOrderBody)
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

        public async Task<string> CaptureOrder(string paypalGivenId)
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
                    var value = await result.Content.ReadAsStringAsync();
                    return value;
                }
                var message = result.Content.ReadAsStringAsync().Result;
                return message;
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

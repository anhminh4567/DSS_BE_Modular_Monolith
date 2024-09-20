using DiamondShop.Domain.Common.ValueObjects;
using FluentResults;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Currencies
{

    public class CurrencyExchangeService
    {
        private static string APP_ID = "647b7d4817d44c9b8c4f16b314200a94";
        private static string VND = "VND";
        private static string USD = "USD";
        public static string Url = "https://openexchangerates.org";
        public async Task<Result> ConvertCurrency(Money source)
        {
            using (var httpClient = new  HttpClient()) 
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization","Token "+APP_ID);
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri($"{Url}/api/convert/{source.Value}/{VND}/{USD}"));
                var response = await httpClient.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync();
                    return Result.Ok();
                }
            }
            return Result.Fail("error");
            //Console.WriteLine("{0}", response.Content);
            //return Result.Ok(Money.CreateVnd(22.2m));
        }
    }
}

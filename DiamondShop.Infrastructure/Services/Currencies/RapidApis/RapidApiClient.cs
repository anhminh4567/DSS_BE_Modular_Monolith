using Azure.Core;
using DiamondShop.Commons;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Infrastructure.Services.Currencies.RapidApis.Models;
using FluentResults;
using System.Net.Http.Json;

namespace DiamondShop.Infrastructure.Services.Currencies.RapidApis
{
    public class RapidApiClient
    {
        private static readonly string x_rapidapi_key = "702a5dbbe1mshe5a65cd179a42cfp1a935bjsn0d9063e03d07";
        private static readonly string x_rapidapi_host = "currency-conversion-and-exchange-rates.p.rapidapi.com";
        private static readonly string DATE_TIME_FORMAT = "yyyy-MM-dd";
        public static async Task<Result<Money>> Convert_FromVND_ToUSD(decimal amount, DateTime? dateToExchange ) 
        {
            if (amount < 0.001m)
                return Result.Fail("amount not valid, must greater than 0.001");
            var moneyVnd = Money.CreateVnd(amount);
            return await Convert(moneyVnd, dateToExchange);
        }
        /// <summary>
        /// should you provide the date to ensure the time of payment and refund are the same amount in that time
        /// avoid inflation, but just a suggestion
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="date"></param>
        /// <returns>
        /// Result of conversion
        /// </returns>
        public static async Task<Result<Money>> Convert_ToVND_FromUSD(decimal amount, DateTime? dateToExchange) 
        {
            if (amount < 0.001m)
                return Result.Fail("amount not valid, must greater than 0.001");
            var moneyVnd = Money.CreateUsd(amount);
            return await Convert(moneyVnd, dateToExchange);
        }
        private static async Task<Result<Money>> Convert(Money moneyToConvert, DateTime? date)
        {
            string currencyConverResult = moneyToConvert.Currency.Equals(Money.VND )
                ? Money.USD 
                : Money.VND;
            string conversionDate = date is null
                ? DateTime.UtcNow.ToString(DATE_TIME_FORMAT)
                : date.Value.ToString(DATE_TIME_FORMAT);
            using (var httpClient = new  HttpClient()) 
            {
                var httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://currency-conversion-and-exchange-rates.p.rapidapi.com/convert?from={moneyToConvert.Currency}&to={currencyConverResult}&amount={moneyToConvert.Value}&date={conversionDate}"),
                    Headers =
                    {
                        { "x-rapidapi-key", x_rapidapi_key },
                        { "x-rapidapi-host", x_rapidapi_host },
                    },
                };
                var response = await httpClient.SendAsync(httpRequest);
                if(response.IsSuccessStatusCode) 
                {
                    var result = await response.Content.ReadFromJsonAsync<RapidApiConvertResponseDetail>();
                    Money moneyResult = Money.Create( currencyConverResult,result.Result);
                    return Result.Ok(moneyResult);
                }
                var stringError = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(stringError);
                return Result.Fail(stringError);
            }
        }
    }
}

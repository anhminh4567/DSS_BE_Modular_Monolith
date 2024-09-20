using DiamondShop.Infrastructure.Services.Currencies.RapidApis;
using Microsoft.AspNetCore.Mvc;
namespace DiamondShop.Api.Controllers.ThirdParties
{

    public record ConversionBody(decimal amount, DateOnly? dateToExchange);
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyExchangeController : ApiControllerBase
    {
        [Route("/ConvertToUSD")]
        [HttpPost]
        public async Task<ActionResult> Convert_ToUSD([FromBody]ConversionBody conversionBody) 
        {
            var result = await RapidApiClient.Convert_FromVND_ToUSD(conversionBody.amount, conversionBody.dateToExchange?.ToDateTime(TimeOnly.MinValue));
            if(result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors,ModelState);
        }
        [Route("/convertToVND")]
        [HttpPost]
        public async Task<ActionResult> Convert_ToVND([FromBody] ConversionBody conversionBody)
        {
            var result = await RapidApiClient.Convert_ToVND_FromUSD(conversionBody.amount, conversionBody.dateToExchange?.ToDateTime(TimeOnly.MinValue));
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
    }
}

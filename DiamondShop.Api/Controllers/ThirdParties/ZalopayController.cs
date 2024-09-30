using DiamondShop.Infrastructure.Services.Payments.Zalopays;
using DiamondShop.Infrastructure.Services.Payments.Zalopays.Models;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.ThirdParties
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZalopayController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ZalopayClient _zalopayClient;

        public ZalopayController(ISender sender, IMapper mapper, IHttpContextAccessor contextAccessor, ZalopayClient zalopayClient)
        {
            _sender = sender;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _zalopayClient = zalopayClient;
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody]ZalopayCreateOrderBody body )
        {
            var result = await ZalopayClient.CreateOrder(body,"http://localhost:7160/swagger/index.html", "https://mk492n6q-7160.asse.devtunnels.ms/Callback");
            return Ok(result);
        }
        [HttpGet("Banks")]
        [Produces(typeof(Dictionary<string, List<ZalopayBankDTO>>))]
        public async Task<ActionResult> GetBanks()
        {
            return Ok(await ZalopayClient.GetBanks());
        }
        [HttpGet("Transaction/{app_transaction_id}")]
        public async Task<ActionResult> GetBanks(string app_transaction_id )
        {
            var result = await ZalopayClient.GetTransactionDetail(app_transaction_id);
            return Ok(result);
        }
        [Route("Callback")]
        [HttpPost()]
        public async Task<ActionResult> Callback()
        {
            var result = await _zalopayClient.Callback();
            return Ok(result);  
        }
    }
}

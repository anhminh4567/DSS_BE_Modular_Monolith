using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.CustomRequest
{
    [Route("api/CustomizeRequest")]
    [ApiController]
    public class CustomizeRequestController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public CustomizeRequestController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("Admin/All")]
        public async Task<ActionResult> GetAll()
        {
            return Ok();
        }
        [HttpPost("Send")]
        public async Task<ActionResult> SendRequest()
        {
            return Ok();
        }
    }
}

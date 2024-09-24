using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Usecases.Diamonds.Commands.Create;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetPaging;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace DiamondShop.Api.Controllers.Diamonds
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiamondController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public DiamondController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetPaging([FromQuery]GetDiamondPagingQuery getDiamondPagingQuery)
        {
            var result = await _sender.Send(getDiamondPagingQuery);
            if(result.IsSuccess) 
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] CreateDiamondCommand createDiamondCommand)
        {
            var result = await _sender.Send(createDiamondCommand);
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
    }
}

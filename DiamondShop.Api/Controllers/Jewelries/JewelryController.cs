using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Application.Usecases.Jewelries.Commands;
using DiamondShop.Application.Usecases.Jewelries.Queries.GetAll;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Application.Usecases.JewelryModels.Queries.GetAll;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Jewelries
{
    [Route("api/Jewelry")]
    [ApiController]
    public class JewelryController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public JewelryController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [Produces(type: typeof(List<JewelryDto>))]
        public async Task<ActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllJewelryQuery());
            var mappedResult = _mapper.Map<List<JewelryDto>>(result);
            return Ok(mappedResult);
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody] CreateJewelryCommand command)
        {
            throw new NotImplementedException();
            var result = await _sender.Send(command);
            //if (result.IsSuccess)
            //{
            //    return Ok(result.Value);
            //}
            //return MatchError(result.Errors, ModelState);
        }

    }
}

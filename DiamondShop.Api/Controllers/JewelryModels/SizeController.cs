using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Application.Usecases.Sizes.Queries.GetAll;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.JewelryModels
{
    [Route("api/Size")]
    [ApiController]
    public class SizeController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        public SizeController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("All")]
        [Produces(type: typeof(List<SizeDto>))]
        public async Task<ActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllSizeQuery());
            var mappedResult = _mapper.Map<List<SizeGroupDto>>(result);
            return Ok(mappedResult);
        }
        public record SizeGroupDto(string Unit, List<SizeDto> Sizes);
    }
}

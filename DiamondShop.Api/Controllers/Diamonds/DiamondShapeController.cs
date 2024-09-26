using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetDetail;
using DiamondShop.Application.Usecases.DiamondShapes.Queries.GetAll;
using DiamondShop.Domain.Models.DiamondShapes;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Diamonds
{
    [Route("api/Diamond/Shape")]
    [ApiController]
    public class DiamondShapeController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public DiamondShapeController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("All")]
        [Produces(type:typeof(List<DiamondShapeDto>))]
        public async Task<ActionResult> GetAll()
        {
            var command = new GetAllDiamondShapeQuery();
            var result = await _sender.Send(command);
            var mappedResult = _mapper.Map<List<DiamondShapeDto>>(result);
            return Ok(mappedResult);
        }
    }
}

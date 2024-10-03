using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Application.Usecases.JewelryModelCategories.Commands.Create;
using DiamondShop.Application.Usecases.JewelryModelCategories.Queries.GetAll;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Application.Usecases.JewelryModels.Queries.GetAll;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.JewelryModels
{
    [Route("api/JewelryModelCategory")]
    [ApiController]
    public class JewelryModelCategoryController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        public JewelryModelCategoryController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("All")]
        [Produces(type: typeof(List<JewelryModelCategoryDto>))]
        public async Task<ActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllCategoryQuery());
            var mappedResult = _mapper.Map<List<JewelryModelCategoryDto>>(result);
            return Ok(mappedResult);
        }
        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody] CreateJewelryCategoryCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}

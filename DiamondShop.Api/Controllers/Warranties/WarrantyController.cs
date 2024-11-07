using DiamondShop.Api.Controllers.Warranties.Delete;
using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Application.Usecases.JewelryModels.Queries.GetDetail;
using DiamondShop.Application.Usecases.Warranties.Commands.Create;
using DiamondShop.Application.Usecases.Warranties.Queries.GetAll;
using DiamondShop.Application.Usecases.Warranties.Queries.GetDetail;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Warranties
{
    [Route("api/Warranty")]
    [ApiController]
    public class WarrantyController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public WarrantyController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("All")]
        public async Task<ActionResult> GetAll([FromQuery] GetAllWarrantyQuery getAllWarrantyQuery)
        {
            var result = await _sender.Send(getAllWarrantyQuery);
            var mappedResult = _mapper.Map<PagingResponseDto<WarrantyDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("Detail")]
        public async Task<ActionResult> GetDetail([FromQuery] GetDetailWarrantyQuery getDetailWarrantyQuery)
        {
            var result = await _sender.Send(getDetailWarrantyQuery);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<WarrantyDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody] CreateWarrantyCommand createWarrantyCommand)
        {
            var result = await _sender.Send(createWarrantyCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<WarrantyDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteWarrantyCommand deleteWarrantyCommand)
        {
            var result = await _sender.Send(deleteWarrantyCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<WarrantyDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}

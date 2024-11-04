using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Application.Usecases.JewelryModels.Queries.GetAll;
using DiamondShop.Application.Usecases.JewelryModels.Queries.GetDetail;
using DiamondShop.Application.Usecases.JewelryModels.Queries.GetSelling;
using DiamondShop.Application.Usecases.JewelryModels.Queries.GetSellingDetail;
using DiamondShop.Application.Usecases.SideDiamonds.Commands.Create;
using DiamondShop.Application.Usecases.SizeMetals.Commands.Create;
using DiamondShop.Application.Usecases.SizeMetals.Commands.Delete;
using DiamondShop.Application.Usecases.SizeMetals.Commands.Update;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.JewelryModels
{
    [Route("api/JewelryModel")]
    [ApiController]
    public class JewelryModelController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        public JewelryModelController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("Staff/All")]
        [Produces(type: typeof(PagingResponseDto<JewelryModelDto>))]
        public async Task<ActionResult> GetAll([FromQuery] GetAllJewelryModelQuery jewelryModelQuery)
        {
            var result = await _sender.Send(jewelryModelQuery);
            var mappedResult = _mapper.Map<PagingResponseDto<JewelryModelDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("Staff/Detail")]
        [Produces(type: typeof(JewelryModelDto))]
        public async Task<ActionResult> GetDetail([FromQuery] string modelId)
        {
            var result = await _sender.Send(new GetJewelryModelDetailQuery(modelId));
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<JewelryModelDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpGet("/Customize/All")]
        [Produces(type: typeof(PagingResponseDto<JewelryModelDto>))]
        public async Task<ActionResult> GetCustomizeAll([FromQuery] GetAllJewelryModelQuery jewelryModelQuery)
        {
            var result = await _sender.Send(jewelryModelQuery);
            var mappedResult = _mapper.Map<PagingResponseDto<JewelryModelDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("/Customize/Detail")]
        [Produces(type: typeof(JewelryModelDto))]
        public async Task<ActionResult> GetCustomizeDetail([FromQuery] string modelId)
        {
            var result = await _sender.Send(new GetJewelryModelDetailQuery(modelId));
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<JewelryModelDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpGet("Selling")]
        [Produces(type: typeof(PagingResponseDto<JewelryModelSellingDto>))]
        public async Task<ActionResult> GetSelling([FromQuery] GetSellingModelQuery getSellingModelQuery)
        {
            var result = await _sender.Send(getSellingModelQuery);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<PagingResponseDto<JewelryModelSellingDto>>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpGet("Selling/Detail")]
        [Produces(type: typeof(JewelryModelSellingDetailDto))]
        public async Task<ActionResult> GetSellingDetail([FromQuery] GetSellingModelDetailQuery getSellingModelDetailQuery)
        {
            var result = await _sender.Send(getSellingModelDetailQuery);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<JewelryModelSellingDetailDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        #region Create Component
        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody] CreateJewelryModelCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<JewelryModelDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("Create/SizeMetal")]
        public async Task<ActionResult> CreateSizeMetal([FromBody] CreateSizeMetalCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                return Ok("Create metal options success");
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("Create/SideDiamondOption")]
        public async Task<ActionResult> CreateSideDiamondOption([FromBody] CreateModelSideDiamondCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                return Ok("Create side diamond(s) success");
            }
            return MatchError(result.Errors, ModelState);
        }
        #endregion
        #region Update Component
        [HttpPut("Update/SizeMetal")]
        public async Task<ActionResult> UpdateSizeMetal([FromForm] UpdateModelSizeMetalCommand updateModelSizeMetalCommand)
        {
            var result = await _sender.Send(updateModelSizeMetalCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<List<SizeMetalDto>>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        #endregion
        #region Remove Component
        [HttpDelete("Delete/SizeMetal")]
        public async Task<ActionResult> DeleteSizeMetal([FromQuery] DeleteModelSizeMetalCommand deleteModelSizeMetalCommand)
        {
            var result = await _sender.Send(deleteModelSizeMetalCommand);
            if (result.IsSuccess)
            {
                return Ok("Delete metal options success");
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("Delete/SideDiamondOption")]
        public async Task<ActionResult> DeleteSideDiamondOption([FromQuery] DeleteModelSizeMetalCommand deleteModelSizeMetalCommand)
        {
            var result = await _sender.Send(deleteModelSizeMetalCommand);
            if (result.IsSuccess)
            {
                return Ok("Delete metal options success");
            }
            return MatchError(result.Errors, ModelState);
        }
        #endregion
    }
}

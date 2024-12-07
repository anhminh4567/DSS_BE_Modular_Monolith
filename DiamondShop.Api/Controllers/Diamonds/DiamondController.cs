using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.CustomizeRequests;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetAll;
using DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetDetail;
using DiamondShop.Application.Usecases.Diamonds.Commands.Create;
using DiamondShop.Application.Usecases.Diamonds.Commands.CreateForCustomizeRequest;
using DiamondShop.Application.Usecases.Diamonds.Commands.Delete;
using DiamondShop.Application.Usecases.Diamonds.Commands.DeletePreOrderDiamondFromCustomizeRequest;
using DiamondShop.Application.Usecases.Diamonds.Commands.LockForUser;
using DiamondShop.Application.Usecases.Diamonds.Queries.DashBoard.GetBestSellingCaratRangeForShape;
using DiamondShop.Application.Usecases.Diamonds.Queries.DashBoard.GetBestSellingForManyShape;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetAll;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetAllAdmin;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetAllAttributes;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetDetail;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetDiamondPricesComparisons;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetFilters;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetLockItemsForUser;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetPaging;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Repositories;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet("FilterLimit")]
        [Produces(typeof(Dictionary<string, Dictionary<string, int>>))]
        public async Task<ActionResult> GetFilterLimit()
        {
            var result = await _sender.Send(new GetDiamondFiltersLimitQuery());
            return Ok(result);
        }
        [HttpGet("AttributesValues")]
        [Produces(typeof(Dictionary<string, Dictionary<string, int>>))]
        public async Task<ActionResult> GetAllAttributeValues()
        {
            var result = await _sender.Send(new GetAllDiamondAttributesQuery());
            return Ok(result);  
        }
        [HttpGet("All")]
        [Produces(typeof(List<DiamondDto>))]
        public async Task<ActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllDiamondQuery());
            var mappedResult = _mapper.Map<List<DiamondDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("LockProduct")]
        [Produces(typeof(List<DiamondDto>))]
        public async Task<ActionResult> GetAllUserLockProduct([FromQuery] GetLockDiamondsForUserQuery getLockDiamondsForUserQuery)
        {
            var result = await _sender.Send(getLockDiamondsForUserQuery);
            var mappedResult = _mapper.Map<List<DiamondDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("All/Admin")]
        [Produces(typeof(List<DiamondDto>))]
        public async Task<ActionResult> GetAllAdmin()
        {
            var result = await _sender.Send(new GetAllDiamondAdminQuery());
            var mappedResult = _mapper.Map<List<DiamondDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("{id}")]
        [Produces(typeof(DiamondDto))]
        public async Task<ActionResult> Get([FromRoute] string id )
        {
            var command = new GetDiamondDetail(id);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiamondDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("EstimatePrice")]
        [Produces(typeof(DiamondDto))]
        public async Task<ActionResult> Estimation(GetDiamondPricesComparisonsQuery getDiamondPricesComparisonsQuery)
        {
            var result = await _sender.Send(getDiamondPricesComparisonsQuery);
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [HttpGet("Page")]
        [Produces(typeof(PagingResponseDto<DiamondDto>))]// diamond paging replaced with this official one
        public async Task<ActionResult> GetPaging([FromQuery] TestGetDiamondPagingQuery getDiamondPagingQuery)
        {
            var result = await _sender.Send(getDiamondPagingQuery);
            if(result.IsSuccess)
            {
                var mappedResult = _mapper.Map<PagingResponseDto<DiamondDto>>(result.Value);
                return Ok(mappedResult);
            }
                
            return MatchError(result.Errors, ModelState);
        }
        //[HttpGet("Page/Test")]
        //[Produces(typeof(PagingResponseDto<DiamondDto>))]
        //public async Task<ActionResult> GetPagingTest([FromQuery] TestGetDiamondPagingQuery query)
        //{
        //    var result = await _sender.Send(query);
        //    if (result.IsSuccess)
        //    {
        //        var mappedResult = _mapper.Map<PagingResponseDto<DiamondDto>>(result.Value);
        //        return Ok(mappedResult);
        //    }
        //    return MatchError(result.Errors, ModelState);
        //}
        [HttpPost]
        [Produces(typeof(DiamondDto))]
        public async Task<ActionResult> Create([FromBody] CreateDiamondRequestDto createDiamondCommand)
        {
            var command = new CreateDiamondCommand(createDiamondCommand.diamond4c,createDiamondCommand.details,
                createDiamondCommand.measurement,createDiamondCommand.shapeId,createDiamondCommand.sku,createDiamondCommand.Certificate,
                createDiamondCommand.priceOffset);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiamondDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("Unavailble")]
        [Produces(typeof(CustomizeRequestDto))]
        public async Task<ActionResult> CreateUnavailable([FromBody] CreateDiamondWhenNotExistCommand createDiamondCommand)
        {
            var result = await _sender.Send(createDiamondCommand);
            if (result.IsSuccess)
            {
                var getDetail = await _sender.Send(new GetDetailCustomizeRequestQuery(createDiamondCommand.customizeRequestId));
                var mappedCustomizeRequest = _mapper.Map<CustomizeRequestDto>(getDetail.Value);
                var mappedResult = _mapper.Map<DiamondDto>(result.Value);
                return Ok(mappedCustomizeRequest);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("Lock")]
        [Produces(typeof(DiamondDto))]
        public async Task<ActionResult> SetLock([FromBody] LockDiamondForUserCommand lockDiamondForUserCommand)
        {
            var result = await _sender.Send(lockDiamondForUserCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiamondDto>(result.Value);
                return Ok(mappedResult);
            }

            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            var command = new DeleteDiamondCommand(id);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("Unavailble")]
        public async Task<ActionResult> DeleteUnavailbe(DeletePreOrderDiamondCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
      
    }
}

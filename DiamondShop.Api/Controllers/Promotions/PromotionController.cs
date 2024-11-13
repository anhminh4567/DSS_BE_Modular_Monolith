using DiamondShop.Application.Dtos.Requests.Carts;
using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Application.Usecases.Discounts.Commands.Cancel;
using DiamondShop.Application.Usecases.PromotionGifts.Commands.CreateMany;
using DiamondShop.Application.Usecases.PromotionGifts.Commands.Delete;
using DiamondShop.Application.Usecases.PromotionGifts.Queries.GetAll;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.Delete;
using DiamondShop.Application.Usecases.PromotionRequirements.Queries.GetAll;
using DiamondShop.Application.Usecases.Promotions.Commands.Cancel;
using DiamondShop.Application.Usecases.Promotions.Commands.Create;
using DiamondShop.Application.Usecases.Promotions.Commands.CreateFull;
using DiamondShop.Application.Usecases.Promotions.Commands.Delete;
using DiamondShop.Application.Usecases.Promotions.Commands.SetThumbnail;
using DiamondShop.Application.Usecases.Promotions.Commands.Update;
using DiamondShop.Application.Usecases.Promotions.Commands.UpdateGifts;
using DiamondShop.Application.Usecases.Promotions.Commands.UpdateInfo;
using DiamondShop.Application.Usecases.Promotions.Commands.UpdateRequirements;
using DiamondShop.Application.Usecases.Promotions.Commands.UpdateStatus;
using DiamondShop.Application.Usecases.Promotions.Queries.GetAll;
using DiamondShop.Application.Usecases.Promotions.Queries.GetApplicablePromotions;
using DiamondShop.Application.Usecases.Promotions.Queries.GetDetail;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Promotions
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public PromotionController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
       
     
        [HttpGet()]
        [ProducesResponseType(typeof(List<PromotionDto>),200)]
        public async Task<ActionResult> GetAllPromotion()
        {
            var result = await _sender.Send(new GetAllPromotionQuery());
            var mappedResult = _mapper.Map<List<PromotionDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("{promotionId}")]
        [ProducesResponseType(typeof(PromotionDto), 200)]
        public async Task<ActionResult> Get([FromRoute] string promotionId)
        {
            var result = await _sender.Send(new GetPromotionDetailQuery(promotionId));
            var mappedResult = _mapper.Map<PromotionDto>(result.Value);
            return Ok(mappedResult);
        }
        [HttpPost("GetApplicable")]
        [ProducesResponseType(typeof(ApplicablePromotionDto), 200)]
        public async Task<ActionResult> GetApplicablePromotion([FromBody] CartRequestDto cartDto)
        {
            var result = await _sender.Send(new GetApplicablePromotionForCartQuery(cartDto));
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost()]
        [ProducesResponseType(typeof(PromotionDto), 200)]
        public async Task<ActionResult> CreatePromotion(CreatePromotionCommand createPromotionCommand)
        {
            var result = await _sender.Send(createPromotionCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<PromotionDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("Full")]
        [ProducesResponseType(typeof(PromotionDto), 200)]
        public async Task<ActionResult> CreatePromotionFull(CreateFullPromotionCommand createFullPromotionCommand)
        {
            var result = await _sender.Send(createFullPromotionCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<PromotionDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("{promotionId}/Requirement")]
        public async Task<ActionResult> UpdatePromotionRequirement([FromRoute]string promotionId, [FromBody] string[] requirementIds, [FromQuery] bool isAdd)
        {
            var result = await _sender.Send(new UpdatePromotionRequirementCommand (promotionId, requirementIds, isAdd));
            if (result.IsSuccess)
            {
                return Ok();
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("{promotionId}/Gift")]
        public async Task<ActionResult> UpdateGiftRequirement([FromRoute]string promotionId, [FromBody] string[] giftIds, [FromQuery]bool isAdd)
        {
            var result = await _sender.Send(new UpdatePromotionGiftsCommand(promotionId, giftIds, isAdd));
            if (result.IsSuccess)
            {
                return Ok();
            }
            return MatchError(result.Errors, ModelState);
        }
        //[HttpPut("{promotionId}")]
        //[ProducesResponseType(typeof(PromotionDto), 200)]
        //public async Task<ActionResult> UpdateInformation([FromRoute] string promotionId, [FromBody] UpdatePromotionInformationCommand updatePromotionInformationCommand)
        //{
        //    var command = updatePromotionInformationCommand with { promotionId = promotionId };
        //    var result = await _sender.Send(command);
        //    if (result.IsSuccess)
        //    {
        //        var mappedResult = _mapper.Map<PromotionDto>(result.Value);
        //        return Ok(mappedResult);
        //    }
        //    return MatchError(result.Errors, ModelState);
        //}
        [HttpPut("{promotionId}")]
        [ProducesResponseType(typeof(PromotionDto), 200)]
        public async Task<ActionResult> UpdatePromotionFull([FromRoute] string promotionId,[FromBody] UpdatePromotionRequest request)
        {
            var result = await _sender.Send(new UpdatePromotionCommand(promotionId,request.UpdatePromotionParams,request.addedRquirements,request.addedGifts,request.removedRequirement,request.removedGifts));
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<PromotionDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPatch("{promotionId}/Pause")]
        [ProducesResponseType(typeof(PromotionDto), 200)]
        public async Task<ActionResult> Pause([FromRoute] PausePromotionCommand pausePromotionCommand)
        {
            var result = await _sender.Send(pausePromotionCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<PromotionDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPatch("{promotionId}/Cancel")]
        [ProducesResponseType(typeof(PromotionDto), 200)]
        public async Task<ActionResult> Cancel([FromRoute] CancelPromotionCommand cancelPromotionCommand)
        {
            var result = await _sender.Send(cancelPromotionCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<PromotionDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("{promotionId}/Thumbnail")]
        public async Task<ActionResult> SetThumbnail([FromRoute] string promotionId, [FromForm] SetPromotionThumbnailCommand setPromotionThumbnailCommand)
        {
            var command = setPromotionThumbnailCommand with { promotionId = promotionId };
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("{promotionId}")]
        [ProducesResponseType(typeof(PromotionDto), 200)]
        public async Task<ActionResult> DeletePromotion(string promotionId)
        {
            var result = await _sender.Send(new DeletePromotionCommand(promotionId));
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<PromotionDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}

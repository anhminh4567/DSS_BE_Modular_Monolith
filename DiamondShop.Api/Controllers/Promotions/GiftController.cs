using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Application.Usecases.PromotionGifts.Commands.CreateMany;
using DiamondShop.Application.Usecases.PromotionGifts.Commands.Delete;
using DiamondShop.Application.Usecases.PromotionGifts.Queries.GetAll;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Promotions
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public GiftController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet()]
        [Produces(typeof(List<GiftDto>))]
        public async Task<ActionResult> GetAllGifts()
        {
            var result = await _sender.Send(new GetAllGiftQuery());
            var mappedResult = _mapper.Map<List<GiftDto>>(result);
            return Ok(mappedResult);
        }
        [HttpPost()]
        [Produces(type: typeof(List<GiftDto>))]
        public async Task<ActionResult> CreateGift(CreateGiftCommand createGiftCommand)
        {
            var result = await _sender.Send(createGiftCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<List<GiftDto>>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("{giftId}")]
        [Produces(type: typeof(GiftDto))]
        public async Task<ActionResult> DeleteGift(string giftId)
        {
            var result = await _sender.Send(new DeleteGiftCommand(giftId));
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<GiftDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}

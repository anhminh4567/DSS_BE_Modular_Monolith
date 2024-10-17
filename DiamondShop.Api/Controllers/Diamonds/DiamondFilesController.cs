using DiamondShop.Application.Usecases.Diamonds.Files.Commands.AddThumbnail;
using DiamondShop.Application.Usecases.Diamonds.Files.Commands.RemoveThumbnail;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Diamonds
{
    [Route("api/Diamond")]
    [ApiController]
    [Tags("Diamond")]
    public class DiamondFilesController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public DiamondFilesController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpPost("{diamondId}/Files/Thumbnail")]
        [Produces(typeof(string))]
        public async Task<ActionResult> UploadThumbnail([FromRoute] string diamondId, IFormFile formFile, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new AddThumbnailCommand(diamondId, formFile), cancellationToken);
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors,ModelState);
        }
        [HttpDelete("{diamondId}/Files/Thumbnail")]
        public async Task<ActionResult> DeleteThumbnail([FromRoute] string diamondId, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new RemoveThumbnailCommand(diamondId), cancellationToken);
            if (result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }
    }
}

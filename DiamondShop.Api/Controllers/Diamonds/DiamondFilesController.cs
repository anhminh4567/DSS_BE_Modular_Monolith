using DiamondShop.Application.Dtos.Responses;
using DiamondShop.Application.Usecases.Diamonds.Files.Commands.AddMany;
using DiamondShop.Application.Usecases.Diamonds.Files.Commands.AddThumbnail;
using DiamondShop.Application.Usecases.Diamonds.Files.Commands.RemoveMany;
using DiamondShop.Application.Usecases.Diamonds.Files.Commands.RemoveThumbnail;
using DiamondShop.Application.Usecases.Diamonds.Files.Queries;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Mozilla;

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
        [HttpGet("{diamondId}/Files")]
        public async Task<ActionResult> GetAllFiles([FromRoute] string diamondId)
        {
            var result = await _sender.Send(new GetAllImagesQuery(diamondId));
            var mappedResult = _mapper.Map<GalleryTemplateDto>(result);
            return Ok(mappedResult);
        }
        [HttpPost("{diamondId}/Files/Thumbnail")]
        [Produces(typeof(string))]
        public async Task<ActionResult> UploadThumbnail([FromRoute] string diamondId, IFormFile formFile, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new AddThumbnailCommand(diamondId, formFile), cancellationToken);
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("{diamondId}/Files/Images")]
        public async Task<ActionResult> UploadImages([FromRoute] string diamondId, IFormFile[] formFiles)
        {
            var result = await _sender.Send(new AddManyImagesCommand(diamondId, formFiles));
            if(result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("{diamondId}/Files/Images")]
        public async Task<ActionResult> DeleteImages([FromRoute] string diamondId, [FromBody]string[] absolutePaths)
        {
            var result = await _sender.Send(new RemoveManyFilesCommand(diamondId, absolutePaths));
            if (result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
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

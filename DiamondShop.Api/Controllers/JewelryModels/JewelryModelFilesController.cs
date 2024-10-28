using DiamondShop.Application.Dtos.Responses;
using DiamondShop.Application.Usecases.Diamonds.Files.Commands.AddCertficate;
using DiamondShop.Application.Usecases.Diamonds.Files.Commands.AddMany;
using DiamondShop.Application.Usecases.Diamonds.Files.Commands.AddThumbnail;
using DiamondShop.Application.Usecases.Diamonds.Files.Commands.RemoveMany;
using DiamondShop.Application.Usecases.Diamonds.Files.Commands.RemoveThumbnail;
using DiamondShop.Application.Usecases.Diamonds.Files.Queries;
using DiamondShop.Application.Usecases.JewelryModels.Files.Commands.AddBaseImages;
using DiamondShop.Application.Usecases.JewelryModels.Files.Commands.AddMetalImages;
using DiamondShop.Application.Usecases.JewelryModels.Files.Commands.AddThumbnail;
using DiamondShop.Application.Usecases.JewelryModels.Files.Commands.RemoveMany;
using DiamondShop.Application.Usecases.JewelryModels.Files.Commands.RemoveThumbnail;
using DiamondShop.Application.Usecases.JewelryModels.Files.Queries;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.JewelryModels
{
    [Route("api/[controller]")]
    [ApiController]
    public class JewelryModelFilesController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public JewelryModelFilesController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("{jewelryModelId}/Files")]
        public async Task<ActionResult> GetAllFiles([FromRoute] string jewelryModelId)
        {
            var result = await _sender.Send(new GetAllModelImagesQuery(jewelryModelId));
            var mappedResult = _mapper.Map<GalleryTemplateDto>(result);
            return Ok(mappedResult);
        }
        [HttpPost("{jewelryModelId}/Files/Thumbnail")]
        [Produces(typeof(string))]
        public async Task<ActionResult> UploadThumbnail([FromRoute] string jewelryModelId, IFormFile formFile, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new AddJewelryThumbnailCommand(jewelryModelId, formFile), cancellationToken);
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("{jewelryModelId}/Files/Images/Base")]
        public async Task<ActionResult> UploadBaseImages([FromRoute] string jewelryModelId,IFormFile[] formFiles)
        {
            var result = await _sender.Send(new AddModelBaseImagesCommand(jewelryModelId, formFiles));
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("{jewelryModelId}/Files/Images/Metals/{metalId}")]
        public async Task<ActionResult> UploadMetalImages([FromRoute] string jewelryModelId,[FromRoute] string metalId, IFormFile[] formFiles)
        {
            var result = await _sender.Send(new AddModelMetalImagesCommand(jewelryModelId,metalId, formFiles));
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("{jewelryModelId}/Files/Images")]
        public async Task<ActionResult> DeleteImages([FromRoute] string jewelryModelId, [FromBody] string[] absolutePaths)
        {
            var result = await _sender.Send(new RemoveManyModelFilesCommand(jewelryModelId, absolutePaths));
            if (result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("{jewelryModelId}/Files/Thumbnail")]
        public async Task<ActionResult> DeleteThumbnail([FromRoute] string jewelryModelId, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new RemoveModelThumbnailCommand(jewelryModelId), cancellationToken);
            if (result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }
    }
}

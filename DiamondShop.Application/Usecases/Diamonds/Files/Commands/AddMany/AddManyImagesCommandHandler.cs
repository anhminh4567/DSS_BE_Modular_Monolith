using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Files.Commands.AddMany
{
    public record AddManyImagesCommand(string diamondId,IFormFile[] images): IRequest<Result<string[]>>;
    internal class AddManyImagesCommandHandler : IRequestHandler<AddManyImagesCommand, Result<string[]>>
    {
        private readonly IDiamondFileService _diamondFileService;
        private readonly ILogger<AddManyImagesCommandHandler> _logger;
        private readonly IDiamondRepository _diamondRepository;

        public AddManyImagesCommandHandler(IDiamondFileService diamondFileService, ILogger<AddManyImagesCommandHandler> logger, IDiamondRepository diamondRepository)
        {
            _diamondFileService = diamondFileService;
            _logger = logger;
            _diamondRepository = diamondRepository;
        }

        public async Task<Result<string[]>> Handle(AddManyImagesCommand request, CancellationToken cancellationToken)
        {
            var parsedId = DiamondId.Parse(request.diamondId);
            var getDiamond = await _diamondRepository.GetById(parsedId);
            if (getDiamond is null)
            {
                return Result.Fail(new NotFoundError("diamond not found"));
            }
            if(request.images.Any(x => FileUltilities.IsImageFileContentType(x.ContentType) == false))
                return Result.Fail(new ConflictError("Contain Invalid file type, has to be image"));
            DiamondFileData[] diamondFileDatas = request.images.Select(x => new DiamondFileData(x.FileName,null,x.ContentType,x.OpenReadStream())).ToArray();
            var uploadedResult = await _diamondFileService.UploadGallery(getDiamond, diamondFileDatas,cancellationToken);
            return uploadedResult;
        }
    }
}

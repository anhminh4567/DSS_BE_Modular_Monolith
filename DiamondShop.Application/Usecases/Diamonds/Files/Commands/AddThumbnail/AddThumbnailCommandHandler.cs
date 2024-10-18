using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
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

namespace DiamondShop.Application.Usecases.Diamonds.Files.Commands.AddThumbnail
{
    public record AddThumbnailCommand(string diamondId, IFormFile FormFile) : IRequest<Result<string>>;

    internal class AddThumbnailCommandHandler : IRequestHandler<AddThumbnailCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondFileService _diamondFileService;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IBlobFileServices _blobFileServices;
        private readonly ILogger<AddThumbnailCommandHandler> _logger;

        public AddThumbnailCommandHandler(IUnitOfWork unitOfWork, IDiamondFileService diamondFileService, IDiamondRepository diamondRepository, IBlobFileServices blobFileServices, ILogger<AddThumbnailCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _diamondFileService = diamondFileService;
            _diamondRepository = diamondRepository;
            _blobFileServices = blobFileServices;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(AddThumbnailCommand request, CancellationToken cancellationToken)
        {
            var parsedId = DiamondId.Parse(request.diamondId);
            Diamond? getDiamond = await _diamondRepository.GetById(parsedId);
            if (getDiamond is null)
            {
                return Result.Fail("Diamond not found");
            }
            var fileExtension = Path.GetExtension(request.FormFile.FileName).Trim();
            var contentType = request.FormFile.ContentType;
            var stream = request.FormFile.OpenReadStream();
            var diamondFileData = new DiamondFileData(request.FormFile.FileName.Replace(fileExtension,""), fileExtension, contentType, stream) ;
            if(getDiamond.Thumbnail != null)
            {
                await _blobFileServices.DeleteFileAsync(getDiamond.Thumbnail.MediaPath, cancellationToken);
                getDiamond.ChangeThumbnail(null);
            }
            var uploadResult = await _diamondFileService.UploadThumbnail(getDiamond, diamondFileData, cancellationToken);
            if (uploadResult.IsSuccess)
            {
                Media thumb = Media.Create(request.FormFile.FileName, uploadResult.Value, contentType);
                getDiamond.ChangeThumbnail(thumb);
                await _diamondRepository.Update(getDiamond);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Ok(_blobFileServices.ToAbsolutePath(uploadResult.Value));
            }
            return uploadResult;
        }
    }
}

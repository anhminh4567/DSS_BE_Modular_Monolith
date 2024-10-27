using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.ValueObjects;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;

namespace DiamondShop.Application.Usecases.JewelryModels.Files.Commands.AddThumbnail
{
    public record AddJewelryThumbnailCommand(string jewelryModelId, IFormFile FormFile) : IRequest<Result<string>>;
    internal class AddJewelryThumbnailCommandHandler : IRequestHandler<AddJewelryThumbnailCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJewelryModelFileService _jewelryModelFileService;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly ILogger<AddJewelryThumbnailCommandHandler> _logger;

        public AddJewelryThumbnailCommandHandler(IUnitOfWork unitOfWork, IJewelryModelFileService jewelryModelFileService, IJewelryModelRepository diamondRepository, ILogger<AddJewelryThumbnailCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _jewelryModelFileService = jewelryModelFileService;
            _jewelryModelRepository = diamondRepository;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(AddJewelryThumbnailCommand request, CancellationToken cancellationToken)
        {
            var parsedId = JewelryModelId.Parse(request.jewelryModelId);
            JewelryModel? getJewelry= await _jewelryModelRepository.GetById(parsedId);
            if (getJewelry is null)
                return Result.Fail("Diamond not found");
            
            var fileExtension = Path.GetExtension(request.FormFile.FileName).Trim();
            var contentType = request.FormFile.ContentType;
            var stream = request.FormFile.OpenReadStream();
            var diamondFileData = new FileData(request.FormFile.FileName.Replace(fileExtension, ""), fileExtension, contentType, stream);
            if (getJewelry.Thumbnail != null)
            {
                await _jewelryModelFileService.DeleteFileAsync(getJewelry.Thumbnail.MediaPath, cancellationToken);
                getJewelry.ChangeThumbnail(null);
            }
            var uploadResult = await _jewelryModelFileService.UploadThumbnail(getJewelry, diamondFileData, cancellationToken);
            if (uploadResult.IsSuccess)
            {
                Media thumb = Media.Create(request.FormFile.FileName, uploadResult.Value, contentType);
                getJewelry.ChangeThumbnail(thumb);
                await _jewelryModelRepository.Update(getJewelry);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Ok(_jewelryModelFileService.ToAbsolutePath(uploadResult.Value));
            }
            return uploadResult;
        }
    }
}

using DiamondShop.Application.Commons.Rules;
using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Commands.SetThumbnail
{
    public record SetPromotionThumbnailCommand(string? promotionId, IFormFile? imageFile) : IRequest<Result>;
    internal class SetPromotionThumbnailCommandHandler : IRequestHandler<SetPromotionThumbnailCommand, Result>
    {
        private readonly IBlobFileServices _blobFileServices;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetPromotionThumbnailCommandHandler(IBlobFileServices blobFileServices, IPromotionRepository promotionRepository, IUnitOfWork unitOfWork)
        {
            _blobFileServices = blobFileServices;
            _promotionRepository = promotionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(SetPromotionThumbnailCommand request, CancellationToken cancellationToken)
        {
            var parsedId = PromotionId.Parse(request.promotionId!);
            var getPromotion = await _promotionRepository.GetById(parsedId);
            if (getPromotion is null)
                return Result.Fail(new NotFoundError());
            // if there is a file, then update, else remove the old image
            if (request.imageFile != null)
            {
                var extension = Path.GetExtension(request.imageFile.FileName);
                var basePath = PromotionImagesPathRules.GetBasePath();
                var fileName = request.imageFile.FileName.Replace(extension, "");
                var contentType = request.imageFile.ContentType;
                if (FileUltilities.IsImageFileContentType(contentType) == false ||
                    FileUltilities.IsImageFileExtension(extension) == false)
                {
                    return Result.Fail("Invalid file type");
                }
                if (getPromotion.Thumbnail != null)
                    await _blobFileServices.DeleteFileAsync(getPromotion.Thumbnail.MediaPath);
                basePath = basePath + PromotionImagesPathRules.GetRandomizedFileName(fileName);
                var stream = request.imageFile.OpenReadStream();
                var uploadResult = await _blobFileServices.UploadFileAsync(basePath, stream, contentType, cancellationToken);
                if (uploadResult.IsSuccess)
                {
                    var thumb = Media.Create(fileName, basePath, contentType);
                    getPromotion.Thumbnail = thumb;
                }
                else
                {
                    return Result.Fail("fail to upload new image");
                }
            }
            else
            {
                getPromotion.Thumbnail = null;
            }
            await _promotionRepository.Update(getPromotion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}

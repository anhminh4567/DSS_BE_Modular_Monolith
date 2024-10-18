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

namespace DiamondShop.Application.Usecases.Discounts.Commands.SetThumbnail
{
    public record SetDiscountThumbnailCommand(string? discountId, IFormFile? imageFile) : IRequest<Result>;
    internal class SetDiscountThumbnailCommandHandler : IRequestHandler<SetDiscountThumbnailCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountRepository _discountRepository;
        private readonly IBlobFileServices _blobFileServices;

        public SetDiscountThumbnailCommandHandler(IUnitOfWork unitOfWork, IDiscountRepository discountRepository, IBlobFileServices blobFileServices)
        {
            _unitOfWork = unitOfWork;
            _discountRepository = discountRepository;
            _blobFileServices = blobFileServices;
        }

        public async Task<Result> Handle(SetDiscountThumbnailCommand request, CancellationToken cancellationToken)
        {
            var parsedId = DiscountId.Parse(request.discountId!);
            var getDiscount = await _discountRepository.GetById(parsedId);
            if (getDiscount is null)
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
                if (getDiscount.Thumbnail != null)
                    await _blobFileServices.DeleteFileAsync(getDiscount.Thumbnail.MediaPath);
                basePath = basePath + PromotionImagesPathRules.GetRandomizedFileName(fileName);
                var stream = request.imageFile.OpenReadStream();
                var uploadResult = await _blobFileServices.UploadFileAsync(basePath, stream, contentType, cancellationToken);
                if (uploadResult.IsSuccess)
                {
                    var thumb = Media.Create(fileName, basePath, contentType);
                    getDiscount.Thumbnail = thumb;
                }
                else
                {
                    return Result.Fail("fail to upload new image");
                }
            }
            else
            {
                getDiscount.Thumbnail = null;
            }
            await _discountRepository.Update(getDiscount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}

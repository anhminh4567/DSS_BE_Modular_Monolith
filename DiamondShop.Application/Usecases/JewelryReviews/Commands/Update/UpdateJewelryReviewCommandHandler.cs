using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.JewelryReviews;
using DiamondShop.Application.Usecases.JewelryReviews.Commands.ChangeVisibility;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryReviewRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryReviews.Commands.Update
{
    public record UpdateJewelryReviewRequestDto(string JewelryId, string Content, int StarRating, IFormFile[] Files);
    public record UpdateJewelryReviewCommand(string AccountId, UpdateJewelryReviewRequestDto UpdateJewelryReviewRequestDto) : IRequest<Result<JewelryReview>>;
    internal class UpdateJewelryReviewCommandHandler : IRequestHandler<UpdateJewelryReviewCommand, Result<JewelryReview>>
    {
        private readonly IJewelryReviewRepository _jewelryReviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJewelryReviewFileService _jewelryReviewFileService;
        public UpdateJewelryReviewCommandHandler(IJewelryReviewRepository jewelryReviewRepository, IUnitOfWork unitOfWork, IJewelryReviewFileService jewelryReviewFileService)
        {
            _jewelryReviewRepository = jewelryReviewRepository;
            _unitOfWork = unitOfWork;
            _jewelryReviewFileService = jewelryReviewFileService;
        }

        public async Task<Result<JewelryReview>> Handle(UpdateJewelryReviewCommand request, CancellationToken token)
        {
            request.Deconstruct(out string accountId, out UpdateJewelryReviewRequestDto updateJewelryReviewRequestDto);
            updateJewelryReviewRequestDto.Deconstruct(out string jewelryId, out string content, out int starRating, out IFormFile[] files);
            await _unitOfWork.BeginTransactionAsync(token);
            var review = await _jewelryReviewRepository.GetById(JewelryId.Parse(jewelryId));
            if (review == null)
                return Result.Fail("This review doesn't exist");
            if (review.AccountId.Value != accountId)
                return Result.Fail("You don't have permission to change this review");
            if (review.Jewelry == null)
                return Result.Fail("Can't get the jewelry of this review");
            var deleteFlag = await _jewelryReviewFileService.DeleteFiles(review.Jewelry);
            if (deleteFlag.IsFailed)
                return Result.Fail(deleteFlag.Errors);

            review.Content = content;
            review.StarRating = starRating;
            await _jewelryReviewRepository.Update(review);
            await _unitOfWork.SaveChangesAsync(token);
            
            if(files.Count() > 0)
            {
                FileData[] fileDatas = files.Select(f => new FileData(f.Name, f.ContentType, f.ContentType, f.OpenReadStream())).ToArray();
                var result = await _jewelryReviewFileService.UploadReview(review.Jewelry, fileDatas);
                if (result.IsFailed)
                    return Result.Fail(result.Errors);
                review.Medias.AddRange(result.Value.Select(p => Media.Create("", p, p.Split(".")[1])));
            }
            await _unitOfWork.CommitAsync(token);
            return review;
        }
    }
}

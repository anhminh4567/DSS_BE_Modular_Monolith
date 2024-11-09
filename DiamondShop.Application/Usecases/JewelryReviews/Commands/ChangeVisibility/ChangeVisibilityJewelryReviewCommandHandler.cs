﻿using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.JewelryReviews;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories.JewelryReviewRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelryReviews.Commands.ChangeVisibility
{
    public record ChangeVisibilityJewelryReviewCommand(string AccountId, string AccountRole, string JewelryId) : IRequest<Result<JewelryReview>>;
    internal class ChangeVisibilityJewelryReviewCommandHandler : IRequestHandler<ChangeVisibilityJewelryReviewCommand, Result<JewelryReview>>
    {
        private readonly IJewelryReviewRepository _jewelryReviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJewelryReviewFileService _jewelryReviewFileService;
        public ChangeVisibilityJewelryReviewCommandHandler(IJewelryReviewRepository jewelryReviewRepository, IUnitOfWork unitOfWork, IJewelryReviewFileService jewelryReviewFileService)
        {
            _jewelryReviewRepository = jewelryReviewRepository;
            _unitOfWork = unitOfWork;
            _jewelryReviewFileService = jewelryReviewFileService;
        }

        public async Task<Result<JewelryReview>> Handle(ChangeVisibilityJewelryReviewCommand request, CancellationToken token)
        {
            request.Deconstruct(out string accountId, out string accountRole, out string jewelryId);
            await _unitOfWork.BeginTransactionAsync(token);
            var review = await _jewelryReviewRepository.GetById(JewelryId.Parse(jewelryId));
            if (review == null)
                return Result.Fail("This review doesn't exist");
            if (review.Jewelry == null)
                return Result.Fail("Can't get the jewelry of this review");
            if (accountRole == AccountRole.CustomerId)
            {
                if(review.AccountId.Value != accountId)
                    return Result.Fail("You don't have permission to remove this review");
                if(review.IsHidden)
                    return Result.Fail("This review has already been removed");
            }
            review.IsHidden = !review.IsHidden;
            await _jewelryReviewRepository.Update(review);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            review.Medias = await _jewelryReviewFileService.GetFolders(review.Jewelry);
            return review;
        }
    }
}

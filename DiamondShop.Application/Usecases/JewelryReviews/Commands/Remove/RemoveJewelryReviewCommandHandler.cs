using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.JewelryReviews;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories.JewelryReviewRepo;
using FluentResults;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryReviews.Commands.Remove
{
    public record RemoveJewelryReviewCommand(string AccountId, string JewelryId) : IRequest<Result>;
    internal class RemoveJewelryReviewCommandHandler : IRequestHandler<RemoveJewelryReviewCommand, Result>
    {
        private readonly IJewelryReviewRepository _jewelryReviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJewelryReviewFileService _jewelryReviewFileService;

        public RemoveJewelryReviewCommandHandler(IJewelryReviewRepository jewelryReviewRepository, IUnitOfWork unitOfWork, IJewelryReviewFileService jewelryReviewFileService)
        {
            _jewelryReviewRepository = jewelryReviewRepository;
            _unitOfWork = unitOfWork;
            _jewelryReviewFileService = jewelryReviewFileService;
        }

        public async Task<Result> Handle(RemoveJewelryReviewCommand request, CancellationToken token)
        {
            request.Deconstruct(out string accountId, out string jewelryId);
            await _unitOfWork.BeginTransactionAsync(token);
            var review = await _jewelryReviewRepository.GetById(JewelryId.Parse(jewelryId));
            if (review == null)
                return Result.Fail(JewelryErrors.Review.ReviewNotFoundError);
            if (review.Jewelry == null)
                return Result.Fail(JewelryErrors.Review.ReviewJewelryNotFoundError);
            var deleteFlag = await _jewelryReviewFileService.DeleteFiles(review.Jewelry);
            if (deleteFlag.IsFailed)
                return Result.Fail(deleteFlag.Errors);
            await _jewelryReviewRepository.Delete(review);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
    }
}

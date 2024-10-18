using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Commands.UpdateGifts
{
    public record UpdatePromotionGiftsCommand(string promotionId, string[] giftId, bool isAdd = true) : IRequest<Result>;
    internal class UpdatePromotionGiftsCommandHandler : IRequestHandler<UpdatePromotionGiftsCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IGiftRepository _giftRepository;

        public UpdatePromotionGiftsCommandHandler(IUnitOfWork unitOfWork, IPromotionRepository promotionRepository, IGiftRepository giftRepository)
        {
            _unitOfWork = unitOfWork;
            _promotionRepository = promotionRepository;
            _giftRepository = giftRepository;
        }

        public async Task<Result> Handle(UpdatePromotionGiftsCommand request, CancellationToken cancellationToken)
        {
            var promoIdParsed = PromotionId.Parse(request.promotionId);
            var giftIdParsed = request.giftId.Select(x => GiftId.Parse(x)).ToList();
            var tryGetPromotion = await _promotionRepository.GetById(promoIdParsed);
            if (tryGetPromotion == null)
                return Result.Fail(new NotFoundError("no promotion with such id"));
            var tryGetGift = await _giftRepository.GetRange(giftIdParsed);
            if (tryGetGift == null || tryGetGift.Any() is false)
                return Result.Fail(new NotFoundError("not found any gift"));
            if (request.isAdd)
            {
                //if (tryGetPromotion.Gifts.Contains(tryGetGift))
                //    return Result.Fail("already have this item, no need to add");
                tryGetGift.ForEach(g => tryGetPromotion.AddGift(g));
            }
            else
                tryGetGift.ForEach(g => tryGetPromotion.RemoveGift(g));

            await _promotionRepository.Update(tryGetPromotion);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}

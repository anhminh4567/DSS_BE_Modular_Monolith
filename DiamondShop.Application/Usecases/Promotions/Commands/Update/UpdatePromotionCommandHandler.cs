using DiamondShop.Application.Dtos.Requests.Promotions;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.PromotionGifts.Commands.CreateMany;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using DiamondShop.Application.Usecases.Promotions.Commands.UpdateGifts;
using DiamondShop.Application.Usecases.Promotions.Commands.UpdateInfo;
using DiamondShop.Application.Usecases.Promotions.Commands.UpdateRequirements;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ErrorMessages;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Commands.Update
{
    public record UpdatePromotionRequest(string? promotionId, string? name, string? description, UpdateStartEndDate? UpdateStartEndDate, List<RequirementSpec>? requirements, List<GiftSpec>? gifts, List<string>? removedRequirements, List<string>? removedGifts) : IRequest<Result<Promotion>>;
    public record UpdatePromotionCommand(string promotionId, UpdatePromotionInformationCommand? UpdatePromotionParams, List<RequirementSpec>? addedRquirements, List<GiftSpec>? addedGifts, List<string>? removedRequirements, List<string>? removedGifts) : IRequest<Result<Promotion>>;
    internal class UpdatePromotionCommandHandler : IRequestHandler<UpdatePromotionCommand, Result<Promotion>>
    {
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePromotionCommandHandler(IMapper mapper, ISender sender, IPromotionRepository promotionRepository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _sender = sender;
            _promotionRepository = promotionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Promotion>> Handle(UpdatePromotionCommand request, CancellationToken cancellationToken)
        {
            var parsedId = PromotionId.Parse(request.promotionId);
            var getPromotion = await _promotionRepository.GetById(parsedId);
            //if(getPromotion.Status != Status.Paused && getPromotion.Status != Status.Scheduled )
            //{
            //    return Result.Fail(new ConflictError("Cannot update promotion, must be scheduled or paused to update"));
            //}
            if(getPromotion == null)
                return Result.Fail(PromotionError.NotFound);

            await _unitOfWork.BeginTransactionAsync();
            if(request.UpdatePromotionParams != null)
            {
                var updatePromoInfoResult = await _sender.Send(request.UpdatePromotionParams, cancellationToken);
                if (updatePromoInfoResult.IsFailed)
                {
                    await _unitOfWork.RollBackAsync();
                    return Result.Fail(updatePromoInfoResult.Errors);
                }
            }
            if(request.removedRequirements != null)
            {
                var removeRequirementCommands = new UpdatePromotionRequirementCommand(request.promotionId, request.removedRequirements.ToArray(), false);
                var removeRequirementResult = await _sender.Send(removeRequirementCommands, cancellationToken);
                if (removeRequirementResult.IsFailed)
                {
                    await _unitOfWork.RollBackAsync();
                    return Result.Fail(removeRequirementResult.Errors);
                }
            }
            if (request.removedGifts != null)
            {
                var removeGiftCommands = new UpdatePromotionGiftsCommand(request.promotionId, request.removedGifts.ToArray(), false);
                var removeGiftResult = await _sender.Send(removeGiftCommands, cancellationToken);
                if (removeGiftResult.IsFailed)
                {
                    await _unitOfWork.RollBackAsync();
                    return Result.Fail(removeGiftResult.Errors);
                }
            }
            List<PromoReq> addedReq = new();
            List<Gift> addedGift= new();
            if (request.addedRquirements != null)
            {
                var createRequirementsResult = await _sender.Send(new CreateRequirementCommand(request.addedRquirements), cancellationToken);
                if (createRequirementsResult.IsFailed)
                {
                    await _unitOfWork.RollBackAsync();
                    return Result.Fail(createRequirementsResult.Errors);
                }
                addedReq = createRequirementsResult.Value;
            }
            if(request.addedGifts != null)
            {
                var createGiftsResult = await _sender.Send(new CreateGiftCommand(request.addedGifts), cancellationToken);
                if (createGiftsResult.IsFailed)
                {
                    await _unitOfWork.RollBackAsync();
                    return Result.Fail(createGiftsResult.Errors);
                }
                addedGift = createGiftsResult.Value;
            }
            addedReq.ForEach(x => getPromotion.AddRequirement(x));
            addedGift.ForEach(x => getPromotion.AddGift(x));
            if(getPromotion.PromoReqs.Where(x => x.TargetType ==TargetType.Order).Count() > 1)
            {
                await _unitOfWork.RollBackAsync();
                return Result.Fail(PromotionError.RequirentTypeLimit(TargetType.Order,1));
            }
            if (getPromotion.Gifts.Where(x => x.TargetType == TargetType.Order).Count() > 1)
            {
                await _unitOfWork.RollBackAsync();
                return Result.Fail(PromotionError.GiftTypeLimit(TargetType.Order, 1));
            }
            if (getPromotion.Gifts.Count == 0)
            {
                await _unitOfWork.RollBackAsync(); 
                return Result.Fail(PromotionError.GiftError.CountIsZero);
            }
                
            if(getPromotion.PromoReqs.Count == 0)
            {
                await _unitOfWork.RollBackAsync();
                return Result.Fail(PromotionError.RequirementError.CountIsZero);
            }
            await _promotionRepository.Update(getPromotion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return getPromotion;
        }
    }
}

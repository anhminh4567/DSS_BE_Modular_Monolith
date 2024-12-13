using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.PromotionGifts.Commands.CreateMany;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using DiamondShop.Application.Usecases.Promotions.Commands.Create;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ErrorMessages;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Commands.CreateFull
{
    public record CreateFullPromotionCommand(CreatePromotionCommand CreatePromotionCommand, List<RequirementSpec> Requirements, List<GiftSpec> Gifts) : IRequest<Result<Promotion>>;
    internal class CreateFullPromotionCommandHandler : IRequestHandler<CreateFullPromotionCommand, Result<Promotion>>
    {
        private readonly IUnitOfWork _unitOfWork;   
        private readonly IPromotionRepository _promotionRepository;
        private readonly ISender _sender;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public CreateFullPromotionCommandHandler(IUnitOfWork unitOfWork, IPromotionRepository promotionRepository, ISender sender, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _unitOfWork = unitOfWork;
            _promotionRepository = promotionRepository;
            _sender = sender;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<Promotion>> Handle(CreateFullPromotionCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var createPromotionResult = await _sender.Send(request.CreatePromotionCommand, cancellationToken);
            if(createPromotionResult.IsFailed)
            {
                await _unitOfWork.RollBackAsync(cancellationToken);
                return Result.Fail(createPromotionResult.Errors);
            }
            var createRequirementsResult = await _sender.Send(new CreateRequirementCommand(request.Requirements), cancellationToken);   
            if(createRequirementsResult.IsFailed)
            {
                await _unitOfWork.RollBackAsync(cancellationToken);
                return Result.Fail(createRequirementsResult.Errors);
            }
            var createGiftsResult = await _sender.Send(new CreateGiftCommand(request.Gifts), cancellationToken);
            if(createGiftsResult.IsFailed)
            {
                await _unitOfWork.RollBackAsync(cancellationToken);
                return Result.Fail(createGiftsResult.Errors);
            }
            var promotion = createPromotionResult.Value;
            var requirements = createRequirementsResult.Value;
            var gifts = createGiftsResult.Value;
            requirements.ForEach(x => promotion.AddRequirement(x));
            gifts.ForEach(x => promotion.AddGift(x));
            var validateReqResult = promotion.ValidateRequirement(_optionsMonitor.CurrentValue.PromotionRule);
            if (validateReqResult.IsFailed)
            {
                await _unitOfWork.RollBackAsync();
                return Result.Fail(validateReqResult.Errors);
            }
            var validateGiftResult = promotion.ValidateGift(_optionsMonitor.CurrentValue.PromotionRule);
            if (validateGiftResult.IsFailed)
            {
                await _unitOfWork.RollBackAsync();
                return Result.Fail(validateGiftResult.Errors);
            }
            await _promotionRepository.Update(promotion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return promotion;
        }
    }
}

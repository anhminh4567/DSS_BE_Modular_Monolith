using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.PromotionGifts.Commands.CreateMany;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using DiamondShop.Application.Usecases.Promotions.Commands.Create;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public CreateFullPromotionCommandHandler(IUnitOfWork unitOfWork, IPromotionRepository promotionRepository, ISender sender)
        {
            _unitOfWork = unitOfWork;
            _promotionRepository = promotionRepository;
            _sender = sender;
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
            await _promotionRepository.Update(promotion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return promotion;
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Commands.UpdateStatus
{
    public record PausePromotionCommand(string promotionId) : IRequest<Result<Promotion>>;
    internal class PausePromotionCommandHandler : IRequestHandler<PausePromotionCommand, Result<Promotion>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IPromotionServices _promotionServices;

        public PausePromotionCommandHandler(IUnitOfWork unitOfWork, IPromotionRepository promotionRepository, IPromotionServices promotionServices)
        {
            _unitOfWork = unitOfWork;
            _promotionRepository = promotionRepository;
            _promotionServices = promotionServices;
        }

        public async Task<Result<Promotion>> Handle(PausePromotionCommand request, CancellationToken cancellationToken)
        {
            var parsedId = PromotionId.Parse(request.promotionId);
            var getPromotion= await _promotionRepository.GetById(parsedId);
            if(getPromotion is null)
                return Result.Fail(new NotFoundError());
            var result = getPromotion.Paused();
            if (result.IsSuccess)
            {
                await _promotionRepository.Update(getPromotion);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return getPromotion;
            }
            else
            {
                return Result.Fail(result.Errors);
            }
        }
    }
}

using DiamondShop.Application.Services.Data;
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

namespace DiamondShop.Application.Usecases.Promotions.Commands.UpdateRequirements
{
    public record UpdatePromotionRequirementCommand(string promotionId,string[] requirementIds, bool isAdd = true) : IRequest<Result>;
    internal class UpdatePromotionRequirementCommandHandler : IRequestHandler<UpdatePromotionRequirementCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IRequirementRepository _requirementRepository;

        public UpdatePromotionRequirementCommandHandler(IUnitOfWork unitOfWork, IPromotionRepository promotionRepository, IRequirementRepository requirementRepository)
        {
            _unitOfWork = unitOfWork;
            _promotionRepository = promotionRepository;
            _requirementRepository = requirementRepository;
        }

        public async Task<Result> Handle(UpdatePromotionRequirementCommand request, CancellationToken cancellationToken)
        {
            var promoIdParsed = PromotionId.Parse(request.promotionId);
            var reqIdsParsed = request.requirementIds.Select(r => PromoReqId.Parse(r)).ToList();
            var tryGetPromotion = await _promotionRepository.GetById(promoIdParsed);
            if (tryGetPromotion == null)
                return Result.Fail(new NotFoundError("no promotion with such id"));
            var tryGetRequirements = await _requirementRepository.GetRange(reqIdsParsed);
            if (tryGetRequirements == null)
                return Result.Fail(new NotFoundError("not found requirement"));
            if (request.isAdd)
            {
                //if (tryGetPromotion.PromoReqs.Contains(tryGetRequirement))
                //    return Result.Fail("already have this item, no need to add");
                tryGetRequirements.ForEach(r => tryGetPromotion.AddRequirement(r));
            }
            else
                tryGetRequirements.ForEach(r => tryGetPromotion.RemoveRequirement(r));

            await _promotionRepository.Update(tryGetPromotion);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}

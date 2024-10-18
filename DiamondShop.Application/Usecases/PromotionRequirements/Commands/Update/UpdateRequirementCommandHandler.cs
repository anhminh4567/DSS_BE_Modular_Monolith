using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.PromotionRequirements.Commands.Update
{
    public record UpdateDiamondRequirementParam(DiamondOrigin? Origin, string[]? ShapesIDs, float caratFrom, float caratTo, Clarity clarityFrom, Clarity clarityTo, Cut cutFrom, Cut cutTo, Color colorFrom, Color colorTo);
    public record UpdateOrderRequirementParam(decimal? amount);
    public record UpdateJewelryRequirementParam(decimal? amount, int? quantity, string? changedModelId);
    public record UpdateRequirementCommand(string? requimentId, string? name, UpdateOrderRequirementParam? UpdateOrderRequirement, UpdateJewelryRequirementParam? UpdateJewelryRequirement, UpdateDiamondRequirementParam? UpdateDiamondRequirement) : IRequest<Result<PromoReq>>;
    internal class UpdateRequirementCommandHandler : IRequestHandler<UpdateRequirementCommand, Result<PromoReq>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequirementRepository _requirementRepository;

        public UpdateRequirementCommandHandler(IUnitOfWork unitOfWork, IRequirementRepository requirementRepository)
        {
            _unitOfWork = unitOfWork;
            _requirementRepository = requirementRepository;
        }

        public Task<Result<PromoReq>> Handle(UpdateRequirementCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}

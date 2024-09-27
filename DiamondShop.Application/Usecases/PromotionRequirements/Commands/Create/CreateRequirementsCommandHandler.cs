using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.PromotionRequirements.Commands.Create
{
    public record DiamondRequirementSpec(DiamondOrigin Origin, string[] ShapesIDs, float caratFrom, float caratTo, Clarity clarityFrom, Clarity clarityTo, Cut cutFrom, Cut cutTo, Color colorFrom, Color colorTo);
    public record RequirementSpec(string Name, TargetType TargetType, Operator Operator, decimal? MoneyAmount, int? Quantity, string JewelryModelID, DiamondRequirementSpec? DiamondRequirementSpec, bool isPromotion = true);
    public record CreateRequirementCommand(List<RequirementSpec> Requirements) : IRequest<Result<List<RequirementSpec>>>;
    public class CreateRequirementsCommandHandler : IRequestHandler<CreateRequirementCommand, Result<List<RequirementSpec>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequirementRepository _requirementRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IPromotionRepository _promotionRepository;

        public CreateRequirementsCommandHandler(IUnitOfWork unitOfWork, IRequirementRepository requirementRepository, IDiamondShapeRepository diamondShapeRepository, IPromotionRepository promotionRepository)
        {
            _unitOfWork = unitOfWork;
            _requirementRepository = requirementRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _promotionRepository = promotionRepository;
        }

        public async Task<Result<List<RequirementSpec>>> Handle(CreateRequirementCommand request, CancellationToken cancellationToken)
        {
            var shapes = await _diamondShapeRepository.GetAll();
            //var requirement = PromoReq.
            throw new NotImplementedException();
        }
    }

}

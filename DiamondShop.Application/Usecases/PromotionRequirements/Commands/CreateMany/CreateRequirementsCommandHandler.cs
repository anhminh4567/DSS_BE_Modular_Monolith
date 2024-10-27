using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany
{
    public record DiamondRequirementSpec(DiamondOrigin Origin, string[] ShapesIDs, float caratFrom, float caratTo, Clarity clarityFrom, Clarity clarityTo, Cut cutFrom, Cut cutTo, Color colorFrom, Color colorTo);
    public record RequirementSpec(string Name, TargetType TargetType, Operator Operator, decimal? MoneyAmount, int? Quantity, string? JewelryModelID, DiamondRequirementSpec? DiamondRequirementSpec, bool isPromotion = true);
    public record CreateRequirementCommand(List<RequirementSpec> Requirements) : IRequest<Result<List<PromoReq>>>;
    public class CreateRequirementsCommandHandler : IRequestHandler<CreateRequirementCommand, Result<List<PromoReq>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequirementRepository _requirementRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;

        public CreateRequirementsCommandHandler(IUnitOfWork unitOfWork, IRequirementRepository requirementRepository, IDiamondShapeRepository diamondShapeRepository, IPromotionRepository promotionRepository, IJewelryModelRepository jewelryModelRepository)
        {
            _unitOfWork = unitOfWork;
            _requirementRepository = requirementRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _promotionRepository = promotionRepository;
            _jewelryModelRepository = jewelryModelRepository;
        }

        public async Task<Result<List<PromoReq>>> Handle(CreateRequirementCommand request, CancellationToken cancellationToken)
        {
            var shapes = await _diamondShapeRepository.GetAll();
            List<PromoReq> requirements = new();
            
            for (int i = 0; i < request.Requirements.Count; i++)
            {
                var req = request.Requirements[i];
                bool isMoneyAmount = false;
                if (req.MoneyAmount is not null)
                    isMoneyAmount = true;
                else if (req.Quantity is not null)
                    isMoneyAmount = false;
                else
                    return Result.Fail(new ConflictError("unspecified money or amount , 1 in 2 must be set,  in the requirement position at : " + (++i)));
                switch (req.TargetType)
                {
                    case TargetType.Jewelry_Model:
                        var jewelryModelId = req.JewelryModelID;
                        if (jewelryModelId == null)
                            return Result.Fail(new ConflictError("no jewelry id found for type jewerlry in the requirement position at : " + (++i)));
                        var jewerlyModelId = JewelryModelId.Parse(jewelryModelId);
                        var jelReq = PromoReq.CreateJewelryRequirement(req.Name, req.Operator, isMoneyAmount, req.MoneyAmount.Value, req.Quantity.Value, jewerlyModelId);
                        requirements.Add(jelReq);
                        break;
                    case TargetType.Diamond:
                        req.DiamondRequirementSpec.Deconstruct(out DiamondOrigin origin, out string[] shapesIDs, out float caratFrom, out float caratTo,
                            out Clarity clarityFrom, out var clarityTo, out Cut cutFrom, out var cutTo, out Color colorFrom, out var colorTo);
                        var shapesIdParsed = req.DiamondRequirementSpec.ShapesIDs.Select(s => DiamondShapeId.Parse(s));
                        var selectedShape = shapes.Where(s => shapesIdParsed.Contains(s.Id)).ToList();
                        var diamondReq = PromoReq.CreateDiamondRequirement(req.Name, req.Operator, isMoneyAmount, req.MoneyAmount, req.Quantity,
                            origin, caratFrom, caratTo, clarityFrom, clarityTo, cutFrom, cutTo, colorFrom, colorTo, selectedShape);
                        requirements.Add(diamondReq);
                        break;
                    case TargetType.Order:
                        if (req.MoneyAmount == null)
                            return Result.Fail("order requirement dont have a money amount specify, error found in the requirement position at : " + (++i));
                        var orderReq = PromoReq.CreateOrderRequirement(req.Name, req.Operator, req.MoneyAmount.Value);
                        requirements.Add(orderReq);
                        break;
                    default:
                        return Result.Fail(new ConflictError("unspecified type found in the requirement position at : " + (++i)));

                }
            }
            if (requirements.Count == 0)
                return Result.Fail(new NotFoundError("nothing to update"));
            var getAnyModelId = requirements.Where(r => r.TargetType == TargetType.Jewelry_Model).Select(x => x.ModelId).ToList();
            if(getAnyModelId.Count > 0)
            {
                var query =  _jewelryModelRepository.GetQuery();
                query = _jewelryModelRepository.QueryFilter(query,x => getAnyModelId.Contains(x.Id));
                var getModels = query.ToList();
                if (getModels.Count != getAnyModelId.Count)
                    return Result.Fail(new NotFoundError("some model id not found"));
            }
            await _requirementRepository.CreateRange(requirements);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(requirements);
        }
    }
}

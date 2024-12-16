using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using DiamondShop.Commons;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.PromotionGifts.Commands.CreateMany
{
    public record DiamondGiftSpec(DiamondOrigin Origin, string[] ShapesIDs, float caratFrom, float caratTo, Clarity clarityFrom, Clarity clarityTo, Cut cutFrom, Cut cutTo, Color colorFrom, Color colorTo);
    public record GiftSpec(string Name, TargetType TargetType, string? ItemCode , UnitType UnitType , decimal UnitValue, DiamondGiftSpec? DiamondRequirementSpec, int Amount = 0, decimal? maxAmount = null);
    public record CreateGiftCommand(List<GiftSpec> giftSpecs) : IRequest<Result<List<Gift>>>;
    internal class CreateGiftCommandHandler : IRequestHandler<CreateGiftCommand, Result<List<Gift>>>
    {
        private readonly IGiftRepository _giftRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public CreateGiftCommandHandler(IGiftRepository giftRepository, IPromotionRepository promotionRepository, IDiamondShapeRepository diamondShapeRepository, IUnitOfWork unitOfWork, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _giftRepository = giftRepository;
            _promotionRepository = promotionRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _unitOfWork = unitOfWork;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<List<Gift>>> Handle(CreateGiftCommand request, CancellationToken cancellationToken)
        {
            var promotionRule = _optionsMonitor.CurrentValue.PromotionRule;
            var shapes = await _diamondShapeRepository.GetAll();
            List<Gift> gifts = new();
            for (int i = 0; i < request.giftSpecs.Count; i++)
            {
                var gift = request.giftSpecs[i];
                switch(gift.TargetType) 
                {
                    case TargetType.Jewelry_Model:
                        var jewerlyModelGift = Gift.CreateJewelry(gift.Name,gift.ItemCode, gift.UnitType,gift.UnitValue,gift.Amount);
                        if(jewerlyModelGift.UnitType == UnitType.Percent && gift.maxAmount != null)
                        {
                            jewerlyModelGift.SetMaxAmount(gift.maxAmount.Value);
                        }
                        gifts.Add(jewerlyModelGift);
                        break;
                    case TargetType.Diamond:
                        gift.DiamondRequirementSpec.Deconstruct(out DiamondOrigin origin, out string[] shapesIDs, out float caratFrom, out float caratTo,
                           out Clarity clarityFrom, out var clarityTo, out Cut cutFrom, out var cutTo, out Color colorFrom, out var colorTo);
                        var selectedShape = shapes.Where(x => shapesIDs.Contains(x.Id.Value)).Select(x => x.Id).ToList();
                        var diamondGift = Gift.CreateDiamond(gift.Name, gift.ItemCode, gift.UnitType, gift.UnitValue, gift.Amount, selectedShape,origin,caratFrom,caratTo,clarityFrom,clarityTo,cutFrom,cutTo,colorFrom,colorTo);
                        if (diamondGift.UnitType == UnitType.Percent && gift.maxAmount != null)
                        {
                            diamondGift.SetMaxAmount(gift.maxAmount.Value);
                        }
                        gifts.Add(diamondGift);
                        break;
                    case TargetType.Order:
                        var orderGift = Gift.CreateOrder(gift.Name,gift.UnitType,gift.UnitValue);
                        if (orderGift.UnitType == UnitType.Percent && gift.maxAmount != null)
                        {
                            orderGift.SetMaxAmount(gift.maxAmount.Value);
                        }
                        if (orderGift.UnitType == UnitType.Percent)
                            if (orderGift.UnitValue > promotionRule.MaxOrderDiscount)
                                return Result.Fail(new Error($"quà loại đơn hàng ở vị trí :{(++i)}; có phần trăm cao hơn cho phép là {promotionRule.MaxOrderDiscount}"));
                        if(orderGift.UnitType == UnitType.Fix_Price)
                            if(orderGift.UnitValue > promotionRule.MaxOrderReducedAmount)
                                return Result.Fail(new Error($"quà loại đơn hàng ở vị trí :{(++i)}; có giá trị cao hơn cho phép là {promotionRule.MaxOrderReducedAmount}"));
                        gifts.Add(orderGift);
                        break;
                    default:
                        return Result.Fail(new ConflictError("không rõ target của gift ở vị trí : " + (++i)));

                }
            }
            if (gifts.Count == 0)
                return Result.Fail(new NotFoundError("không có gì để tạo"));
            await _giftRepository.CreateRange(gifts);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(gifts);
            //throw new NotImplementedException();
        }
    }
    
}

using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace DiamondShop.Application.Usecases.SideDiamonds.Commands.Create
{
    public record CreateModelSideDiamondCommand(string ModelId, SideDiamondRequestDto SideDiamondSpec) : IRequest<Result<SideDiamondOpt>>;
    internal class CreateModelSideDiamondCommandHandler : IRequestHandler<CreateModelSideDiamondCommand, Result<SideDiamondOpt>>
    {
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        public CreateModelSideDiamondCommandHandler(
            ISideDiamondRepository sideDiamondRepository,
            IUnitOfWork unitOfWork,
            IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _sideDiamondRepository = sideDiamondRepository;
            _unitOfWork = unitOfWork;
            _optionsMonitor = optionsMonitor;
        }
        public async Task<Result<SideDiamondOpt>> Handle(CreateModelSideDiamondCommand request, CancellationToken token)
        {
            var rule = _optionsMonitor.CurrentValue.JewelryModelRules;
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out string modelId, out SideDiamondRequestDto sideDiamondSpecs);
            var sides = await _sideDiamondRepository.GetByModelId(JewelryModelId.Parse(modelId));
            if (sides == null || sides.Count() == 0)
                return Result.Fail(JewelryModelErrors.SideDiamond.ModelUnsupportedError);
            if (sides.Count() == rule.MaximumSideDiamondOption)
                return Result.Fail(JewelryModelErrors.SideDiamond.SideDiamondOptAlreadyExistError);
            sideDiamondSpecs.Deconstruct(out string shapeId, out Color colorMin, out Color colorMax, out Clarity clarityMin, out Clarity clarityMax, out SettingType settingType, out float caratWeight, out int quantity, out bool isLabDiamond);
            var existFlag = await _sideDiamondRepository.CheckExist(JewelryModelId.Parse(modelId), DiamondShapeId.Parse(shapeId), caratWeight, quantity, clarityMin, clarityMax, colorMin, colorMax, settingType, isLabDiamond);
            if (existFlag)
                return Result.Fail(JewelryModelErrors.SideDiamond.SideDiamondOptAlreadyExistError);
            var sideDiamond = SideDiamondOpt.Create(JewelryModelId.Parse(modelId), DiamondShapeId.Parse(shapeId), colorMin, colorMax, clarityMin, clarityMax, settingType, caratWeight, quantity, isLabDiamond);
            await _sideDiamondRepository.Create(sideDiamond, token);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return sideDiamond;
        }
    }
}

using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.ModelSideDiamonds.Commands
{
    public record CreateModelSideDiamondCommand(JewelryModelId ModelId, SideDiamondRequestDto SideDiamondSpecs) : IRequest<Result<SideDiamondOpt>>;
    internal class CreateModelSideDiamondCommandHandler : IRequestHandler<CreateModelSideDiamondCommand, Result<SideDiamondOpt>>
    {
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateModelSideDiamondCommandHandler(
            ISideDiamondRepository sideDiamondRepository,
            IUnitOfWork unitOfWork
            )
        {
            _sideDiamondRepository = sideDiamondRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<SideDiamondOpt>> Handle(CreateModelSideDiamondCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out JewelryModelId modelId, out SideDiamondRequestDto sideDiamondSpec);
            sideDiamondSpec.Deconstruct(out string ShapeId, out Color colorMin, out Color colorMax, out Clarity clarityMin, out Clarity clarityMax, out SettingType settingType, out float caratWeight, out int quantity);
            var sideDiamond = SideDiamondOpt.Create(modelId, DiamondShapeId.Parse(ShapeId), colorMin, colorMax, clarityMin, clarityMax, settingType, caratWeight, quantity);
            await _sideDiamondRepository.Create(sideDiamond, token);
            await _unitOfWork.SaveChangesAsync(token);
            return sideDiamond;
        }
    }
}

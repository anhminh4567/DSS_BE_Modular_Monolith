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

namespace DiamondShop.Application.Usecases.SideDiamonds.Commands.Create
{
    public record CreateModelSideDiamondCommand(string ModelId, SideDiamondRequestDto SideDiamondSpec) : IRequest<Result<SideDiamondOpt>>;
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
            request.Deconstruct(out string modelId, out SideDiamondRequestDto sideDiamondSpecs);
            var sideDiamond = SideDiamondOpt.Create(JewelryModelId.Parse(modelId), DiamondShapeId.Parse(sideDiamondSpecs.ShapeId), sideDiamondSpecs.ColorMin, sideDiamondSpecs.ColorMax, sideDiamondSpecs.ClarityMin, sideDiamondSpecs.ClarityMax, sideDiamondSpecs.SettingType, sideDiamondSpecs.CaratWeight, sideDiamondSpecs.Quantity);
            await _sideDiamondRepository.Create(sideDiamond, token);
            await _unitOfWork.SaveChangesAsync(token);
            return sideDiamond;
        }
    }
}

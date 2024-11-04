using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelryModels.Commands.Create
{
    public record CreateJewelryModelCommand(JewelryModelRequestDto ModelSpec, List<MainDiamondRequestDto>? MainDiamondSpecs, List<SideDiamondRequestDto>? SideDiamondSpecs, List<ModelMetalSizeRequestDto> MetalSizeSpecs) : IRequest<Result<JewelryModel>>;
    internal class CreateJewelryModelCommandHandler : IRequestHandler<CreateJewelryModelCommand, Result<JewelryModel>>
    {
        private readonly ISender _sender;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IJewelryModelCategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateJewelryModelCommandHandler(
            ISender sender,
            IJewelryModelCategoryRepository categoryRepository,
            IJewelryModelRepository jewelryModelRepository,
        IUnitOfWork unitOfWork,
        ISizeMetalRepository sizeMetalRepository,
        ISideDiamondRepository sideDiamondRepository,
        IMainDiamondRepository mainDiamondRepository)
        {
            _sender = sender;
            _jewelryModelRepository = jewelryModelRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _sizeMetalRepository = sizeMetalRepository;
            _sideDiamondRepository = sideDiamondRepository;
            _mainDiamondRepository = mainDiamondRepository;
        }
        public async Task<Result<JewelryModel>> Handle(CreateJewelryModelCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out JewelryModelRequestDto modelSpec, out List<MainDiamondRequestDto>? mainDiamondSpecs,
                out List<SideDiamondRequestDto>? sideDiamondSpecs, out List<ModelMetalSizeRequestDto> metalSizeSpecs);
            var category = _categoryRepository.GetQuery().FirstOrDefault(p => p.Id == JewelryModelCategoryId.Parse(modelSpec.CategoryId));
            if (category is null) return Result.Fail(new NotFoundError("Can't find model category object."));

            var matchingName = _jewelryModelRepository.GetQuery().Any(p => p.Name.Trim().ToUpper() == modelSpec.Name.Trim().ToUpper());
            if (matchingName) return Result.Fail("This model name has already existed");
            var matchingCode = _jewelryModelRepository.IsExistModelCode(modelSpec.Code);
            if (matchingName) return Result.Fail("This model code has already existed");
            var newModel = JewelryModel.Create(modelSpec.Name, modelSpec.Code.ToUpper(), category.Id, modelSpec.Width, modelSpec.Length, modelSpec.IsEngravable, modelSpec.IsRhodiumFinish, modelSpec.BackType, modelSpec.ClaspType, modelSpec.ChainType);
            await _jewelryModelRepository.Create(newModel, token);

            var sizeMetals = metalSizeSpecs.Select(p => SizeMetal.Create(newModel.Id, MetalId.Parse(p.MetalId), SizeId.Parse(p.SizeId), p.Weight)).ToList();
            await _sizeMetalRepository.CreateRange(sizeMetals, token);
            await _unitOfWork.SaveChangesAsync(token);

            if (mainDiamondSpecs != null)
            {
                foreach (var mainDiamondSpec in mainDiamondSpecs)
                {
                    var mainDiamond = MainDiamondReq.Create(newModel.Id, mainDiamondSpec.SettingType, mainDiamondSpec.Quantity);
                    await _mainDiamondRepository.Create(mainDiamond, token);
                    if (mainDiamondSpec.ShapeSpecs.Count > 0)
                    {
                        List<MainDiamondShape> mainDiamondShapes = mainDiamondSpec.ShapeSpecs.Select(p => MainDiamondShape.Create(mainDiamond.Id, DiamondShapeId.Parse(p.ShapeId), p.CaratFrom, p.CaratTo)).ToList();
                        await _mainDiamondRepository.CreateRange(mainDiamondShapes, token);
                    }
                    await _unitOfWork.SaveChangesAsync(token);
                    newModel.MainDiamonds.Add(mainDiamond);
                }
            }
            if (sideDiamondSpecs != null)
            {
                var sideDiamonds = sideDiamondSpecs.Select(p => SideDiamondOpt.Create(newModel.Id, DiamondShapeId.Parse(p.ShapeId), p.ColorMin, p.ColorMax, p.ClarityMin, p.ClarityMax, p.SettingType, p.CaratWeight, p.Quantity)).ToList();
                await _sideDiamondRepository.CreateRange(sideDiamonds, token);
                await _unitOfWork.SaveChangesAsync(token);
            }
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            newModel.Category = category;
            return newModel;
        }
    }
}

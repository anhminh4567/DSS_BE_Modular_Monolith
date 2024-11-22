using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Services.interfaces;
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
        private readonly IDiamondServices _diamondServices;

        public CreateJewelryModelCommandHandler(ISender sender, ISizeMetalRepository sizeMetalRepository, IMainDiamondRepository mainDiamondRepository, ISideDiamondRepository sideDiamondRepository, IJewelryModelRepository jewelryModelRepository, IJewelryModelCategoryRepository categoryRepository, IUnitOfWork unitOfWork, IDiamondServices diamondServices)
        {
            _sender = sender;
            _sizeMetalRepository = sizeMetalRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _sideDiamondRepository = sideDiamondRepository;
            _jewelryModelRepository = jewelryModelRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _diamondServices = diamondServices;
        }

        public async Task<Result<JewelryModel>> Handle(CreateJewelryModelCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out JewelryModelRequestDto modelSpec, out List<MainDiamondRequestDto>? mainDiamondSpecs,
                out List<SideDiamondRequestDto>? sideDiamondSpecs, out List<ModelMetalSizeRequestDto> metalSizeSpecs);
            var category = await _categoryRepository.GetById(JewelryModelCategoryId.Parse(modelSpec.CategoryId));
            if (category is null) return Result.Fail(JewelryModelErrors.Category.JewelryModelCategoryNotFoundError);

            var matchingName = _jewelryModelRepository.IsExistModelName(modelSpec.Name);
            if (matchingName) return Result.Fail(JewelryModelErrors.ExistedModelNameFound(modelSpec.Name));
            var matchingCode = _jewelryModelRepository.IsExistModelCode(modelSpec.Code);
            if (matchingCode) return Result.Fail(JewelryModelErrors.ExistedModelCodeFound(modelSpec.Code));
            var newModel = JewelryModel.Create(modelSpec.Name, modelSpec.Code.ToUpper(), category.Id, modelSpec.craftmanFee, modelSpec.Width, modelSpec.Length, modelSpec.IsEngravable, modelSpec.IsRhodiumFinish, modelSpec.BackType, modelSpec.ClaspType, modelSpec.ChainType);
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
                var sideDiamonds = sideDiamondSpecs.Select(p => SideDiamondOpt.Create(newModel.Id, DiamondShapeId.Parse(p.ShapeId), p.ColorMin, p.ColorMax, p.ClarityMin, p.ClarityMax, p.SettingType, p.CaratWeight, p.Quantity, p.IsLabDiamond)).ToList();

                foreach (var sideDiamond in sideDiamonds)
                {
                    var isInAnyCriteria = await _diamondServices.IsSideDiamondFoundInCriteria(sideDiamond);
                    if (isInAnyCriteria == false)
                    {
                        int index = sideDiamonds.IndexOf(sideDiamond);
                        return Result.Fail(JewelryModelErrors.SideDiamond.NoCriteriaFound(index));
                    }
                }
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

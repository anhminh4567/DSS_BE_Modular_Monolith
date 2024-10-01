using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Commands.Create
{
    public record ModelMetalSizeSpec(string MetalId, string SizeId);
    public record CreateJewelryModelCommand(ModelSpec ModelSpec, List<MainDiamondSpec> MainDiamondSpecs, List<SideDiamondSpec> SideDiamondSpecs, List<ModelMetalSizeSpec> MetalSizeSpecs) : IRequest<Result<JewelryModel>>;
    internal class CreateJewelryModelCommandHandler : IRequestHandler<CreateJewelryModelCommand, Result<JewelryModel>>
    {
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateJewelryModelCommandHandler(
            IJewelryModelRepository jewelryModelRepository, 
            IMainDiamondRepository mainDiamondRepository,
            ISideDiamondRepository sideDiamondRepository,
            ISizeMetalRepository sizeMetalRepository, IUnitOfWork unitOfWork)
        {
            _jewelryModelRepository = jewelryModelRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _sideDiamondRepository = sideDiamondRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<JewelryModel>> Handle(CreateJewelryModelCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out ModelSpec ModelSpec, out List< MainDiamondSpec> MainDiamondSpecs,
                out List< SideDiamondSpec> SideDiamondSpecs, out List<ModelMetalSizeSpec> MetalSizeSpecs);
            var newModel = JewelryModel.Create(ModelSpec.Name, JewelryModelCategoryId.Parse(ModelSpec.CategoryId), ModelSpec.Width, ModelSpec.Length, ModelSpec.IsEngravable, ModelSpec.IsRhodiumFinish, ModelSpec.BackType, ModelSpec.ClaspType, ModelSpec.ChainType);
            await _jewelryModelRepository.Create(newModel, token);
            
            List<SizeMetal> listSizeMetal = MetalSizeSpecs.Select(p => SizeMetal.Create(newModel.Id, MetalId.Parse(p.MetalId), SizeId.Parse(p.SizeId))).ToList();
            await _sizeMetalRepository.CreateRange(listSizeMetal, token);

            foreach(var mainDiamondSpec in MainDiamondSpecs)
            {
                var mainDiamond = MainDiamondReq.Create(newModel.Id, mainDiamondSpec.SettingType, mainDiamondSpec.Quantity);
                await _mainDiamondRepository.Create(mainDiamond, token);

                List<MainDiamondShape> mainDiamondShapes = mainDiamond.Shapes.Select(p => MainDiamondShape.Create(mainDiamond.Id, p.ShapeId, p.CaratFrom, p.CaratTo)).ToList();
                await _mainDiamondRepository.CreateShapes(mainDiamondShapes, token);
            }
            foreach (var sideDiamondSpec in SideDiamondSpecs)
            {
                var sideDiamond = SideDiamondReq.Create(newModel.Id, DiamondShapeId.Parse(sideDiamondSpec.ShapeId), sideDiamondSpec.ColorMin, sideDiamondSpec.ColorMax, sideDiamondSpec.ClarityMin, sideDiamondSpec.ClarityMax, sideDiamondSpec.SettingType);
                await _sideDiamondRepository.Create(sideDiamond, token);

                List<SideDiamondOpt> sideDiamondOpts = sideDiamondSpec.OptSpecs.Select(p => SideDiamondOpt.Create(sideDiamond.Id, p.CaratWeight, p.Quantity)).ToList();
                await _sideDiamondRepository.CreateOpts(sideDiamondOpts, token);
            }
            await _unitOfWork.SaveChangesAsync();
            return newModel;
        }
    }
}

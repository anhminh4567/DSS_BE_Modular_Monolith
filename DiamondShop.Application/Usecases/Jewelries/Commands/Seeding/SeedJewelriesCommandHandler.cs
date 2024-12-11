using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Newtonsoft.Json.Linq;

namespace DiamondShop.Application.Usecases.Jewelries.Commands.Seeding
{
    #region model
    public record SeedModelsCommand(int metalTake = 2, int minSize = 10, int maxSize = 12) : IRequest<Result>;
    internal class SeedModelsCommandHandler : IRequestHandler<SeedModelsCommand, Result>
    {
        private readonly ISender _sender;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IMetalRepository _metalRepository;
        private readonly ISizeRepository _sizeRepository;
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJewelryService _jewelryService;


        public SeedModelsCommandHandler(ISender sender, IUnitOfWork unitOfWork, IMetalRepository metalRepository, ISizeRepository sizeRepository, IJewelryModelRepository jewelryModelRepository, ISizeMetalRepository sizeMetalRepository, IMainDiamondRepository mainDiamondRepository, ISideDiamondRepository sideDiamondRepository, IJewelryService jewelryService, IJewelryRepository jewelryRepository, IDiamondRepository diamondRepository, IDiamondShapeRepository diamondShapeRepository)
        {
            _sender = sender;
            _unitOfWork = unitOfWork;
            _metalRepository = metalRepository;
            _sizeRepository = sizeRepository;
            _jewelryModelRepository = jewelryModelRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _sideDiamondRepository = sideDiamondRepository;
            _jewelryService = jewelryService;
            _jewelryRepository = jewelryRepository;
            _diamondRepository = diamondRepository;
            _diamondShapeRepository = diamondShapeRepository;
        }

        public async Task<Result> Handle(SeedModelsCommand request, CancellationToken token)
        {
            request.Deconstruct(out int metalTake, out int minSize, out int maxSize);
            //Seed jewelry model
            var metals = await _metalRepository.GetAll();
            var sizes = await _sizeRepository.GetAll();
            var diamondShapes = await _diamondShapeRepository.GetAll();
            float defaultWeight = 10;
            List<ModelMetalSizeRequestDto> sizeMetalSpecs = new List<ModelMetalSizeRequestDto>();
            foreach (var metal in metals.Take(metalTake))
            {
                var takenSize = sizes.Where(p => p.Value >= minSize && p.Value <= maxSize);
                sizeMetalSpecs.AddRange(takenSize.Select(p => new ModelMetalSizeRequestDto(metal.Id.Value, p.Id.Value, defaultWeight + p.Value)));
            }
            List<CreateJewelryModelCommand> modelCommands = new()
            {
                new(FullDiamondRing[0],OneMainDiamond,ThreeSideDiamond,sizeMetalSpecs),
                new(FullDiamondRing[1],OneMainDiamond,ThreeSideDiamond,sizeMetalSpecs),
                new(FullDiamondRing[2],OneMainDiamond,ThreeSideDiamond,sizeMetalSpecs),
                new(FullDiamondRing[3],ThreeMainDiamond,OneSideDiamond,sizeMetalSpecs),
                //new(NoDiamondRing[0],null,null,sizeMetalSpecs),
                //new(NoDiamondRing[1],null,null,sizeMetalSpecs),
                //new(SideDiamondRing[0],null,ThreeSideDiamond,sizeMetalSpecs),
                //new(SideDiamondRing[1],null,FiveSideDiamond,sizeMetalSpecs),
            };
            await _unitOfWork.BeginTransactionAsync(token);
            foreach (var command in modelCommands)
            {
                var existCodeFlag = _jewelryModelRepository.ExistingModelCode(command.ModelSpec.Code.ToUpper());
                if (existCodeFlag)
                    return Result.Fail("Đã tồn tại");
                var model = JewelryModel.Create(command.ModelSpec.Name, command.ModelSpec.Code.ToUpper(), JewelryModelCategoryId.Parse(command.ModelSpec.CategoryId), command.ModelSpec.craftmanFee, command.ModelSpec.Width, command.ModelSpec.Length, command.ModelSpec.IsEngravable, command.ModelSpec.BackType, command.ModelSpec.ClaspType, command.ModelSpec.ChainType);
                await _jewelryModelRepository.Create(model);
                await _unitOfWork.SaveChangesAsync(token);

                var sizeMetals = command.MetalSizeSpecs.Select(p => SizeMetal.Create(model.Id, MetalId.Parse(p.MetalId), SizeId.Parse(p.SizeId), p.Weight)).ToList();
                await _sizeMetalRepository.CreateRange(sizeMetals, token);

                if (command.MainDiamondSpecs != null)
                {
                    foreach (var mainDiamondSpec in command.MainDiamondSpecs)
                    {
                        var mainDiamond = MainDiamondReq.Create(model.Id, mainDiamondSpec.SettingType, mainDiamondSpec.Quantity);
                        await _mainDiamondRepository.Create(mainDiamond, token);
                        if (mainDiamondSpec.ShapeSpecs.Count > 0)
                        {
                            List<MainDiamondShape> mainDiamondShapes = mainDiamondSpec.ShapeSpecs.Select(p => MainDiamondShape.Create(mainDiamond.Id, DiamondShapeId.Parse(p.ShapeId), p.CaratFrom, p.CaratTo)).ToList();
                            await _mainDiamondRepository.CreateRange(mainDiamondShapes, token);
                        }
                        await _unitOfWork.SaveChangesAsync(token);
                    }
                }

                if (command.SideDiamondSpecs != null)
                {
                    var sideDiamonds = command.SideDiamondSpecs.Select(p => SideDiamondOpt.Create(model.Id, DiamondShapeId.Parse(p.ShapeId), p.ColorMin, p.ColorMax, p.ClarityMin, p.ClarityMax, p.SettingType, p.CaratWeight, p.Quantity, p.IsLabDiamond)).ToList();

                    foreach (var sideDiamond in sideDiamonds)
                    {
                        var existFlag = await _sideDiamondRepository.CheckExist(sideDiamond.ModelId, sideDiamond.ShapeId, sideDiamond.CaratWeight, sideDiamond.Quantity, sideDiamond.ClarityMin, sideDiamond.ClarityMax, sideDiamond.ColorMin, sideDiamond.ColorMax, sideDiamond.SettingType, sideDiamond.IsLabGrown);
                        if (existFlag)
                            return Result.Fail(JewelryModelErrors.SideDiamond.SideDiamondOptAlreadyExistError);
                    }
                    await _sideDiamondRepository.CreateRange(sideDiamonds, token);
                    await _unitOfWork.SaveChangesAsync(token);
                }
                //Seed Jewelries
                foreach (var sizeMetal in model.SizeMetals)
                {
                    if (model.SideDiamonds.Count > 0)
                    {
                        foreach (var sideDiamond in model.SideDiamonds)
                        {
                            var serialCode = await _jewelryService.GetSerialCode(model, sizeMetal.Metal, sizeMetal.Size);
                            var jewelry = Jewelry.Create
                              (
                                  model.Id,
                                  sizeMetal.SizeId,
                                  sizeMetal.MetalId,
                                  sizeMetal.Weight,
                                  serialCode,
                                  status: ProductStatus.Active
                              );
                            var jewelrySideDiamond = JewelrySideDiamond.Create(sideDiamond);
                            jewelry.SideDiamond = jewelrySideDiamond;
                            await _jewelryRepository.Create(jewelry, token);
                            await _unitOfWork.SaveChangesAsync(token);
                            var diamondsResult = await CreateJewelryDiamond(model, jewelry, diamondShapes);
                            if (diamondsResult.IsFailed)
                                return Result.Fail(diamondsResult.Errors);
                        }
                    }
                    else
                    {
                        var serialCode = await _jewelryService.GetSerialCode(model, sizeMetal.Metal, sizeMetal.Size);
                        var jewelry = Jewelry.Create
                          (
                              model.Id,
                              sizeMetal.SizeId,
                              sizeMetal.MetalId,
                              sizeMetal.Weight,
                              serialCode,
                              status: ProductStatus.Active
                          );
                        await _jewelryRepository.Create(jewelry, token);
                        await _unitOfWork.SaveChangesAsync(token);
                        var diamondsResult = await CreateJewelryDiamond(model, jewelry, diamondShapes);
                        if (diamondsResult.IsFailed)
                            return Result.Fail(diamondsResult.Errors);
                    }
                }
            }
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
        async Task<Result<List<Diamond>>> CreateJewelryDiamond(JewelryModel model, Jewelry jewelry, List<DiamondShape> diamondShapes)
        {
            int quantity = model.MainDiamonds.Sum(p => p.Quantity);
            List<Diamond> diamonds = new();
            for (int j = 0; j < quantity; j++)
            {
                var mainDiamond = model.MainDiamonds[j];
                var mainShape = mainDiamond.Shapes[0];
                var diamondShape = diamondShapes.FirstOrDefault(p => p.Id == mainShape.ShapeId);
                Diamond_4C diamond4C = new Diamond_4C(Cut.Good, Color.F, Clarity.VVS2, mainShape.CaratFrom, false);
                Diamond_Details diamondDetail = new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.None, Culet.None);
                Diamond_Measurement diamondMeasurement = new Diamond_Measurement(1f, 1f, 1f, "1x1");
                Diamond newDiamond = Diamond.Create(diamondShape, diamond4C, diamondDetail, diamondMeasurement, 0m, null);
                await _diamondRepository.Create(newDiamond);
                await _unitOfWork.SaveChangesAsync();
                newDiamond.SetForJewelry(jewelry);
                await _diamondRepository.Update(newDiamond);
                await _unitOfWork.SaveChangesAsync();
                diamonds.Add(newDiamond);
            }
            return diamonds;
        }
        public List<JewelryModelRequestDto> FullDiamondRing = new()
        {
            new JewelryModelRequestDto("Nhẫn kim cương Dominatus", "DN", "1", 300_000M, 1.0f,null,true,true,null,null,null),
            new JewelryModelRequestDto("Nhẫn hột xoàn cao cấp Purus", "PUR", "1", 400_000M, 1.5f,null,true,true,null,null,null),
            new JewelryModelRequestDto("Nhẫn kim cương cầu hôn Eternity", "ET", "1", 500_000M, 1.8f,null,true,true,null,null,null),
            new JewelryModelRequestDto("Nhẫn xoàn sang trọng Rafaela", "RAF", "1", 600_000M, 2.0f,null,true,true,null,null,null),
        };
        public List<JewelryModelRequestDto> NoDiamondRing = new()
        {
            new JewelryModelRequestDto("Nhẫn trơn Sabina", "SAB", "1", 700_000M, 1.7f,null,true,true,null,null,null),
            new JewelryModelRequestDto("Nhẫn trơn Olwen", "OLWEN", "1", 800_000M, 1.6f,null,true,true,null,null,null),
        };
        public List<JewelryModelRequestDto> SideDiamondRing = new()
        {
            new JewelryModelRequestDto("Nhẫn kim cương tấm", "SAB", "1", 700_000M, 1.7f,null,true,true,null,null,null),
            new JewelryModelRequestDto("Nhẫn kim cương tấm Solitare", "OLWEN", "1", 800_000M, 1.6f,null,true,true,null,null,null),
        };

        public List<MainDiamondRequestDto> OneMainDiamond
        {
            get
            {
                return new()
        {
            new MainDiamondRequestDto(SettingType.Prong,1,MainDiamondShapeAll)
        };
            }
        }
        public List<MainDiamondRequestDto> TwoMainDiamond
        {
            get
            {
                return new()
        {
            new MainDiamondRequestDto(SettingType.Prong,1,MainDiamondShapeAll),
            new MainDiamondRequestDto(SettingType.Bezel,1,MainDiamondShapeAll),
        };
            }
        }
        public List<MainDiamondRequestDto> ThreeMainDiamond
        {
            get
            {
                return new()
        {
            new MainDiamondRequestDto(SettingType.Prong,1,MainDiamondShapeAll),
            new MainDiamondRequestDto(SettingType.Bezel,1,MainDiamondShapeAll),
            new MainDiamondRequestDto(SettingType.Flush,1,MainDiamondShapeAll),
        };
            }
        }
        public List<MainDiamondShapeRequestDto> MainDiamondShapeAll = new()
        {
            new(DiamondShape.ROUND.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MaxCaratRange),
            new(DiamondShape.PRINCESS.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MaxCaratRange),
            new(DiamondShape.CUSHION.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MaxCaratRange),
            new(DiamondShape.EMERALD.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MaxCaratRange),
            new(DiamondShape.OVAL.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MaxCaratRange),
            new(DiamondShape.RADIANT.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MaxCaratRange),
            new(DiamondShape.ASSCHER.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MaxCaratRange),
            new(DiamondShape.MARQUISE.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MaxCaratRange),
            new(DiamondShape.HEART.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MaxCaratRange),
            new(DiamondShape.PEAR.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MaxCaratRange),
        };
        public List<MainDiamondShapeRequestDto> MainDiamondShapeMinimal = new()
        {
            new(DiamondShape.ROUND.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MinCaratRange+0.2f),
            new(DiamondShape.PRINCESS.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MinCaratRange+0.2f),
            new(DiamondShape.CUSHION.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MinCaratRange+0.2f),
        };
        public List<SideDiamondRequestDto> OneSideDiamond = new()
        {
            new(DiamondShape.ROUND.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.35f, 10,false)
        };
        public List<SideDiamondRequestDto> ThreeSideDiamond = new()
        {
            new(DiamondShape.ROUND.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.35f, 10,false),
            new(DiamondShape.OVAL.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.45f, 15,false),
            new(DiamondShape.OVAL.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.48f, 12,false),
        };
        public List<SideDiamondRequestDto> FiveSideDiamond = new()
        {
            new(DiamondShape.ROUND.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.35f, 10,false),
            new(DiamondShape.ROUND.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.45f, 15,false),
            new(DiamondShape.ROUND.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.48f, 12,false),
            new(DiamondShape.OVAL.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.35f, 10,false),
            new(DiamondShape.OVAL.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.45f, 15,false),
        };
    }
    #endregion
}

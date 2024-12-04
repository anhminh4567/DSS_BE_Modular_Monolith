using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Commands.Seeding
{
    public record SeedJewelriesCommand() : IRequest<Result>;
    internal class SeedJewelriesCommandHandler : IRequestHandler<SeedJewelriesCommand, Result>
    {
        private readonly ISender _sender;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IMetalRepository _metalRepository;
        private readonly ISizeRepository _sizeRepository;
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SeedJewelriesCommandHandler(ISender sender, IUnitOfWork unitOfWork, IMetalRepository metalRepository, ISizeRepository sizeRepository, IJewelryModelRepository jewelryModelRepository, ISizeMetalRepository sizeMetalRepository, IMainDiamondRepository mainDiamondRepository, ISideDiamondRepository sideDiamondRepository)
        {
            _sender = sender;
            _unitOfWork = unitOfWork;
            _metalRepository = metalRepository;
            _sizeRepository = sizeRepository;
            _jewelryModelRepository = jewelryModelRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _sideDiamondRepository = sideDiamondRepository;
        }

        public async Task<Result> Handle(SeedJewelriesCommand request, CancellationToken token)
        {
            throw new NotImplementedException();
            //Seed data
            //var metals = await _metalRepository.GetAll();
            //var sizes = await _sizeRepository.GetAll();
            //float defaultWeight = 10;
            //List<ModelMetalSizeRequestDto> sizeMetalSpecs = new List<ModelMetalSizeRequestDto>();
            //foreach (var metal in metals)
            //{
            //    sizeMetalSpecs.AddRange(sizes.Skip(5).Take(10).Select(p => new ModelMetalSizeRequestDto(metal.Id.Value, p.Id.Value, defaultWeight + p.Value)));
            //}
            //List<CreateJewelryModelCommand> commands = new()
            //{
            //    new(FullDiamondRing[0],OneMainDiamond,ThreeSideDiamond,sizeMetalSpecs),
            //    new(FullDiamondRing[1],OneMainDiamond,ThreeSideDiamond,sizeMetalSpecs),
            //    new(FullDiamondRing[2],TwoMainDiamond,ThreeSideDiamond,sizeMetalSpecs),
            //    new(FullDiamondRing[3],ThreeMainDiamond,OneSideDiamond,sizeMetalSpecs),
            //    new(NoDiamondRing[0],null,null,sizeMetalSpecs),
            //    new(NoDiamondRing[1],null,null,sizeMetalSpecs),
            //    new(SideDiamondRing[0],null,ThreeSideDiamond,sizeMetalSpecs),
            //    new(SideDiamondRing[1],null,FiveSideDiamond,sizeMetalSpecs),
            //};
            //await _unitOfWork.BeginTransactionAsync(token);
            //foreach (var command in commands)
            //{
            //    var model = JewelryModel.Create(command.ModelSpec.Name, command.ModelSpec.Code.ToUpper(), JewelryModelCategoryId.Parse(command.ModelSpec.CategoryId), command.ModelSpec.craftmanFee, command.ModelSpec.Width, command.ModelSpec.Length, command.ModelSpec.IsEngravable, command.ModelSpec.IsRhodiumFinish, command.ModelSpec.BackType, command.ModelSpec.ClaspType, command.ModelSpec.ChainType);
            //    await _jewelryModelRepository.Create(model);
            //    await _unitOfWork.SaveChangesAsync(token);

            //    var sizeMetals = command.MetalSizeSpecs.Select(p => SizeMetal.Create(model.Id, MetalId.Parse(p.MetalId), SizeId.Parse(p.SizeId), p.Weight)).ToList();
            //    await _sizeMetalRepository.CreateRange(sizeMetals, token);

            //    if (command.MainDiamondSpecs != null)
            //    {
            //        foreach (var mainDiamondSpec in command.MainDiamondSpecs)
            //        {
            //            var mainDiamond = MainDiamondReq.Create(newModel.Id, mainDiamondSpec.SettingType, mainDiamondSpec.Quantity);
            //            await _mainDiamondRepository.Create(mainDiamond, token);
            //            if (mainDiamondSpec.ShapeSpecs.Count > 0)
            //            {
            //                List<MainDiamondShape> mainDiamondShapes = mainDiamondSpec.ShapeSpecs.Select(p => MainDiamondShape.Create(mainDiamond.Id, DiamondShapeId.Parse(p.ShapeId), p.CaratFrom, p.CaratTo)).ToList();
            //                await _mainDiamondRepository.CreateRange(mainDiamondShapes, token);
            //            }
            //            await _unitOfWork.SaveChangesAsync(token);
            //            newModel.MainDiamonds.Add(mainDiamond);
            //        }
            //    }
            //}
            //await _unitOfWork.SaveChangesAsync(token);
            //await _unitOfWork.CommitAsync(token);
        }

        private List<JewelryModelRequestDto> FullDiamondRing = new()
        {
            new JewelryModelRequestDto("Nhẫn kim cương Dominatus", "DN", "1", 300_000M, 1.0f,null,true,true,null,null,null),
            new JewelryModelRequestDto("Nhẫn hột xoàn cao cấp Purus", "PUR", "1", 400_000M, 1.5f,null,true,true,null,null,null),
            new JewelryModelRequestDto("Nhẫn kim cương cầu hôn Eternity", "ET", "1", 500_000M, 1.8f,null,true,true,null,null,null),
            new JewelryModelRequestDto("Nhẫn xoàn sang trọng Rafaela", "RAF", "1", 600_000M, 2.0f,null,true,true,null,null,null),
        };
        private List<JewelryModelRequestDto> NoDiamondRing = new()
        {
            new JewelryModelRequestDto("Nhẫn trơn Sabina", "SAB", "1", 700_000M, 1.7f,null,true,true,null,null,null),
            new JewelryModelRequestDto("Nhẫn trơn Olwen", "OLWEN", "1", 800_000M, 1.6f,null,true,true,null,null,null),
        };
        private List<JewelryModelRequestDto> SideDiamondRing = new()
        {
            new JewelryModelRequestDto("Nhẫn kim cương tấm", "SAB", "1", 700_000M, 1.7f,null,true,true,null,null,null),
            new JewelryModelRequestDto("Nhẫn kim cương tấm Solitare", "OLWEN", "1", 800_000M, 1.6f,null,true,true,null,null,null),
        };

        private List<MainDiamondRequestDto> OneMainDiamond
        {
            get
            {
                return new()
                {
                    new MainDiamondRequestDto(SettingType.Prong,1,MainDiamondShapeAll)
                };
            }
        }
        private List<MainDiamondRequestDto> TwoMainDiamond
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
        private List<MainDiamondRequestDto> ThreeMainDiamond
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
        private List<MainDiamondShapeRequestDto> MainDiamondShapeAll = new()
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
        private List<MainDiamondShapeRequestDto> MainDiamondShapeMinimal = new()
        {
            new(DiamondShape.ROUND.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MinCaratRange+0.2f),
            new(DiamondShape.PRINCESS.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MinCaratRange+0.2f),
            new(DiamondShape.CUSHION.Id.Value,(float)DiamondRule.Default.MinCaratRange,(float)DiamondRule.Default.MinCaratRange+0.2f),
        };
        private List<SideDiamondRequestDto> OneSideDiamond = new()
        {
            new(DiamondShape.ROUND.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.35f, 10,false)
        };
        private List<SideDiamondRequestDto> ThreeSideDiamond = new()
        {
            new(DiamondShape.ROUND.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.35f, 10,false),
            new(DiamondShape.OVAL.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.45f, 15,false),
            new(DiamondShape.OVAL.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.48f, 12,false),
        };
        private List<SideDiamondRequestDto> FiveSideDiamond = new()
        {
            new(DiamondShape.ROUND.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.35f, 10,false),
            new(DiamondShape.ROUND.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.45f, 15,false),
            new(DiamondShape.ROUND.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.48f, 12,false),
            new(DiamondShape.OVAL.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.35f, 10,false),
            new(DiamondShape.OVAL.Id.Value,Color.K,Color.F,Clarity.S12,Clarity.VVS2,SettingType.Prong,0.45f, 15,false),
        };
    }
}

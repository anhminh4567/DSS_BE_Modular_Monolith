using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Application.Usecases.MainDiamonds.Commands.Create;
using DiamondShop.Application.Usecases.SideDiamonds.Commands;
using DiamondShop.Application.Usecases.SizeMetals.Commands.Create;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo;
using DiamondShop.Test.General;
using FluentAssertions;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;

namespace DiamondShop.Test.Application.JewelryModels.Create
{
    public class ModelTest
    {
        public Modelspec modelSpec { get; set; }
        public Maindiamondspec[]? mainDiamondSpecs { get; set; }
        public Sidediamondspec[]? sideDiamondSpecs { get; set; }
        public Metalsizespec[] metalSizeSpecs { get; set; }
    }

    public class Modelspec
    {
        public string name { get; set; }
        public string categoryId { get; set; }
        public int? width { get; set; }
        public int? length { get; set; }
        public bool isEngravable { get; set; }
        public bool isRhodiumFinish { get; set; }
        public BackType? backType { get; set; }
        public ClaspType? claspType { get; set; }
        public ChainType? chainType { get; set; }
    }

    public class Maindiamondspec
    {
        public Shapespec[] shapeSpecs { get; set; }
        public SettingType settingType { get; set; }
        public int quantity { get; set; }
    }

    public class Shapespec
    {
        public string shapeId { get; set; }
        public float caratFrom { get; set; }
        public float caratTo { get; set; }
    }

    public class Sidediamondspec
    {
        public string shapeId { get; set; }
        public Color colorMin { get; set; }
        public Color colorMax { get; set; }
        public Clarity clarityMin { get; set; }
        public Clarity clarityMax { get; set; }
        public SettingType settingType { get; set; }
        public Optspec[] optSpecs { get; set; }
    }

    public class Optspec
    {
        public float caratWeight { get; set; }
        public int quantity { get; set; }
    }

    public class Metalsizespec
    {
        public string metalId { get; set; }
        public string sizeId { get; set; }
        public int weight { get; set; }
    }
  
    [Trait(nameof(JewelryModels), "Model")]
    public class CreateModelTest
    {
        private readonly Mock<ISender> _sender;
        private readonly Mock<IJewelryModelRepository> _modelRepo;
        private readonly Mock<IJewelryModelCategoryRepository> _categoryRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;

        private List<MainDiamondShapeRequestDto> mainShapes;
        private MainDiamondRequestDto mainSpec;
        List<SideDiamondOptRequestDto> sideOpts;
        SideDiamondRequestDto sideSpec;
        ModelMetalSizeRequestDto sizeMetalSpec;
        public CreateModelTest()
        {
            _sender = new Mock<ISender>();
            _modelRepo = new Mock<IJewelryModelRepository>();
            _categoryRepo = new Mock<IJewelryModelCategoryRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            mainShapes = new()
                {
                    new MainDiamondShapeRequestDto("1",0.3f, 0.5f),
                };
            mainSpec = new(SettingType.Prong, 0, mainShapes);

            sideOpts = new()
                {
                    new SideDiamondOptRequestDto(2.4f,2),
                };
            sideSpec = new("1", Color.K, Color.D, Clarity.S11, Clarity.S11, SettingType.Prong, sideOpts);

            sizeMetalSpec = new("1", "1", 1);
        }
        [Fact]
        public async Task Handle_Should_CallAllNestedCommands()
        {
            JewelryModelRequestDto modelSpec = new("Test_Ring", "1", null, null, true, true, null, null, null);
            var command = new CreateJewelryModelCommand(modelSpec, new() { mainSpec }, new() { sideSpec }, new() { sizeMetalSpec });
            var handler = new CreateJewelryModelCommandHandler(_sender.Object, _categoryRepo.Object, _modelRepo.Object, _unitOfWork.Object);

            _sender.Setup(s => s.Send(It.IsAny<CreateSizeMetalCommand>(), default)).ReturnsAsync(Result.Ok());
            _sender.Setup(s => s.Send(It.IsAny<CreateMainDiamondCommand>(), default)).ReturnsAsync(Result.Ok());
            _sender.Setup(s => s.Send(It.IsAny<CreateSideDiamondCommand>(), default)).ReturnsAsync(Result.Ok());

            var result = await handler.Handle(command, default);


            _sender.Verify(x => x.Send(It.IsAny<CreateSizeMetalCommand>(), default), Times.Once);
            _sender.Verify(x => x.Send(It.IsAny<CreateMainDiamondCommand>(), default), Times.Once);
            _sender.Verify(x => x.Send(It.IsAny<CreateSideDiamondCommand>(), default), Times.Once);
            _modelRepo.Verify(x => x.Create(It.Is<JewelryModel>(p => p.Id == result.Value.Id), default), Times.Once);
        }
      
    }
}

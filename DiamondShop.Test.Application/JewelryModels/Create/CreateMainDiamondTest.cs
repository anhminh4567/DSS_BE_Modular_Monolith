using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.MainDiamonds.Commands.Create;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;

namespace DiamondShop.Test.Application.JewelryModels.Create
{
    public class MainDiamondJSON
    {
        public MainDiamondShapeJSON[] shapeSpecs { get; set; }
        public int settingType { get; set; }
        public int quantity { get; set; }
    }

    public class MainDiamondShapeJSON
    {
        public string shapeId { get; set; }
        public float caratFrom { get; set; }
        public float caratTo { get; set; }
    }
    [Trait(nameof(JewelryModels), "Main Diamond")]
    public class CreateMainDiamondTest
    {
       

        private readonly Mock<IMainDiamondRepository> _mainDiamondRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        public CreateMainDiamondTest()
        {
            _mainDiamondRepo = new Mock<IMainDiamondRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
        }
        /*      [Fact]
              public async Task Handle_Should_ReturnFailure_WhenQuantityIsBelowOrLowerThan0()
              {
                  List<MainDiamondShapeRequestDto> shapes = new()
                  {
                      new MainDiamondShapeRequestDto("1",0.3f, 0.5f),
                  };
                  MainDiamondSpec spec = new(shapes,SettingType.Prong,0);
                  var command = new CreateMainDiamondCommand(JewelryModelId.Create(), spec);
                  var handler = new CreateMainDiamondCommandHandler(_mainDiamondRepo.Object, _unitOfWork.Object);

                  var result = await handler.Handle(command, default);

                  result.IsFailed.Should().BeTrue();

              }*/
        [Fact]
        public void Handle_Should_ReturnFailure_WhenQuantityIsBelowOrLowerThan0()
        {
            List<MainDiamondShapeRequestDto> shapes = new()
                {
                    new MainDiamondShapeRequestDto("1",0.3f, 0.5f),
                };
            MainDiamondRequestDto spec = new(shapes, SettingType.Prong, 0);
            var command = new CreateMainDiamondCommand(JewelryModelId.Create(), spec);
            var validator = new CreateMainDiamondCommandValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "MainDiamondSpec.Quantity" && e.ErrorMessage == "'Quantity' must be greater than '0'.");
        }
        [Fact]
        public void Handle_Should_ReturnFailure_WhenOneShapeHasNegativeOr0Carat()
        {
            List<MainDiamondShapeRequestDto> shapes = new()
                {
                    new MainDiamondShapeRequestDto("1",-0.03f, -0.5f),
                };
            MainDiamondRequestDto spec = new(shapes, SettingType.Prong, 1);
            var command = new CreateMainDiamondCommand(JewelryModelId.Create(), spec);
            var validator = new CreateMainDiamondCommandValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "MainDiamondSpec.ShapeSpecs[0].CaratFrom" && e.ErrorMessage == "'Carat From' must be greater than '0'.");
            result.Errors.Should().ContainSingle(e => e.PropertyName == "MainDiamondSpec.ShapeSpecs[0].CaratTo");
        }

        [Fact]
        public void Handle_Should_ReturnFailure_WhenCaratFromIsGreaterThanCaratTo()
        {
            List<MainDiamondShapeRequestDto> shapes = new()
                {
                    new MainDiamondShapeRequestDto("1",0.8f, 0.5f),
                };
            MainDiamondRequestDto spec = new(shapes, SettingType.Prong, 1);
            var command = new CreateMainDiamondCommand(JewelryModelId.Create(), spec);
            var validator = new CreateMainDiamondCommandValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "MainDiamondSpec.ShapeSpecs[0].CaratTo");
        }

        [Fact]
        public void Handle_Should_ReturnSuccess_WhenCaratFromIsSmallerThanCaratTo()
        {
            List<MainDiamondShapeRequestDto> shapes = new()
                {
                    new MainDiamondShapeRequestDto("1",0.3f, 0.5f),
                };
            MainDiamondRequestDto spec = new(shapes, SettingType.Prong, 1);
            var command = new CreateMainDiamondCommand(JewelryModelId.Create(), spec);
            var validator = new CreateMainDiamondCommandValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

    }
}

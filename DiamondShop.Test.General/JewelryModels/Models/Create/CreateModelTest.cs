using DiamondShop.Api.Controllers.JewelryModels;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Application.Usecases.MainDiamonds.Commands.Create;
using DiamondShop.Application.Usecases.SideDiamonds.Commands;
using DiamondShop.Application.Usecases.SizeMetals.Commands.Create;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Infrastructure.Databases;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.General.JewelryModels.Models.Create
{
    public class CreateModelTest
    {
        private readonly Mock<ISender> _sender;
        private readonly Mock<IJewelryModelRepository> _modelRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;

        private List<MainDiamondShapeSpec> mainShapes;
        private MainDiamondSpec mainSpec;
        List<SideDiamondOptSpec> sideOpts;
        SideDiamondSpec sideSpec;
        ModelMetalSizeRequestDto sizeMetalSpec;
        public CreateModelTest()
        {
            _sender = new Mock<ISender>();
            _modelRepo = new Mock<IJewelryModelRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            mainShapes = new()
                {
                    new MainDiamondShapeSpec("1",0.3f, 0.5f),
                };
            mainSpec = new(mainShapes, SettingType.Prong, 0);

            sideOpts = new()
                {
                    new SideDiamondOptSpec(2.4f,2),
                };
            sideSpec = new("1", Color.K, Color.D, Clarity.S11, Clarity.S11, SettingType.Prong, sideOpts);

            sizeMetalSpec = new("1", "1", 1);
        }
        [Fact]
        public async Task Handle_Should_CallAllNestedCommands()
        {
            ModelSpec modelSpec = new("Test_Ring", "1", null, null, true, true, null, null, null);
            var command = new CreateJewelryModelCommand(modelSpec, new() { mainSpec }, new() { sideSpec }, new() { sizeMetalSpec });
            var handler = new CreateJewelryModelCommandHandler(_sender.Object, _modelRepo.Object, _unitOfWork.Object);

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

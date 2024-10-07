using DiamondShop.Api.Controllers.JewelryModels;
using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Application.Usecases.MainDiamonds.Commands.Create;
using DiamondShop.Application.Usecases.SideDiamonds.Commands;
using DiamondShop.Application.Usecases.SizeMetals.Commands.Create;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo;
using FluentAssertions;
using FluentResults;
using FluentValidation;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DiamondShop.Test.General.JewelryModels.MainDiamonds.Create.CreateMainDiamondTest;

namespace DiamondShop.Test.General.JewelryModels.Models.Create
{
    public class CreateModelTest
    {

        public class JewelryModelJSON()
        {
            public JewelryModelRequestDto ModelSpec { get; set; }
            public List<MainDiamondRequestDto> MainDiamondSpecs { get; set; }
            public List<SideDiamondRequestDto> SideDiamondSpecs { get; set; }
            public List<ModelMetalSizeRequestDto> MetalSizeSpecs { get; set; }
        }


        private readonly Mock<ISender> _sender;
        private readonly Mock<IJewelryModelRepository> _modelRepo;
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
            _unitOfWork = new Mock<IUnitOfWork>();
            mainShapes = new()
                {
                    new MainDiamondShapeRequestDto("1",0.3f, 0.5f),
                };
            mainSpec = new(mainShapes, SettingType.Prong, 0);

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
        public static IEnumerable<object[]> GetTestData()
        {
            var jsonData = File.ReadAllText("Data/InputModel.json");
            var data = JsonConvert.DeserializeObject<List<JewelryModelJSON>>(jsonData);
            foreach (var item in data)
            {
                yield return new object[] { item.ModelSpec, item.MainDiamondSpecs, item.SideDiamondSpecs, item.MetalSizeSpecs };
            }
        }
        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Handle_Should_ReturnSuccess_WhenModelAddToDb(JewelryModelRequestDto ModelSpec, List<MainDiamondRequestDto> mainDiamondSpecs, List<SideDiamondRequestDto> sideDiamondSpecs, List<ModelMetalSizeRequestDto> metalSizeSpecs)
        {
            var modelValidator = new CreateJewelryModelCommandValidator();
            var sizeMetalValidator = new CreateSizeMetalCommandValidator();
            var mainDiamondValidator = new CreateMainDiamondCommandValidator();
            var sideDiamondValidator = new CreateSideDiamondCommandValidator();



            DbContextOptions opt = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase($"MainDiamondTest {new Guid().ToString()}")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            using (var context = new TestDbContext(opt))
            {

                var unitOfWork = new UnitOfWork(context);
                var modelCommand = new CreateJewelryModelCommand(ModelSpec, mainDiamondSpecs, sideDiamondSpecs, metalSizeSpecs);

                var handler = new CreateJewelryModelCommandHandler(_sender.Object, new JewelryModelRepository(context, null), unitOfWork);
                var sizeMetalHandler = new CreateSizeMetalCommandHandler(new SizeMetalRepository(context), unitOfWork);
                var mainDiamondHandler = new CreateMainDiamondCommandHandler(new MainDiamondRepository(context), unitOfWork);
                var sideDiamondHandler = new CreateSideDiamondCommandHandler(new SideDiamondRepository(context), unitOfWork);

                _sender
                    .Setup(s => s.Send(It.IsAny<CreateSizeMetalCommand>(), It.IsAny<CancellationToken>()))
                    .Returns(async (CreateSizeMetalCommand command, CancellationToken token) =>
                    {
                        var validate = await sizeMetalValidator.ValidateAsync(command, token);
                        if (validate.IsValid)
                            return await sizeMetalHandler.Handle(command, token);
                        else
                            throw new ValidationException(validate.ToString());
                    });
                _sender
                    .Setup(s => s.Send(It.IsAny<CreateMainDiamondCommand>(), It.IsAny<CancellationToken>()))
                    .Returns(async (CreateMainDiamondCommand command, CancellationToken token) =>
                    {
                        var validate = await mainDiamondValidator.ValidateAsync(command, token);
                        if (validate.IsValid)
                            return await mainDiamondHandler.Handle(command, token);
                        else
                            throw new ValidationException(validate.ToString());
                    });
                _sender
                    .Setup(s => s.Send(It.IsAny<CreateSideDiamondCommand>(), It.IsAny<CancellationToken>()))
                    .Returns(async (CreateSideDiamondCommand command, CancellationToken token) =>
                    {
                        var validate = await sideDiamondValidator.ValidateAsync(command, token);
                        if (validate.IsValid)
                            return await sideDiamondHandler.Handle(command, token);
                        else
                            throw new ValidationException(validate.ToString());
                    });
                var result = await handler.Handle(modelCommand, default);

                result.IsSuccess.Should().BeTrue();

                var added = context.JewelryModels.AsQueryable();
                added.Should().HaveCount(1);
            }
        }
    }
}

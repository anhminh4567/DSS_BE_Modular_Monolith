using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.Diamonds.Commands.AttachToJewelry;
using DiamondShop.Application.Usecases.Jewelries.Commands;
using DiamondShop.Application.Usecases.JewelryModelCategories.Commands.Create;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Application.Usecases.JewelrySideDiamonds.Create;
using DiamondShop.Application.Usecases.MainDiamonds.Commands.CompareDiamondShape;
using DiamondShop.Application.Usecases.MainDiamonds.Commands.Create;
using DiamondShop.Application.Usecases.SideDiamonds.Commands;
using DiamondShop.Application.Usecases.SizeMetals.Commands.Create;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Repositories;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryRepo;
using DiamondShop.Test.Application.Jewelries.Create;
using DiamondShop.Test.Application.JewelryModels.Create;
using DiamondShop.Test.General;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DiamondShop.Test.Application
{
    [CollectionDefinition("Database Collection")]
    public class DbCollection : ICollectionFixture<TestDbFixture> { }

    [Collection("Database Collection")]
    public class DbTest
    {
        private readonly Mock<ISender> _sender;
        private readonly TestDbFixture _fixture;
        private readonly TestDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JewelryModelCategoryRepository _categoryRepository;
        private readonly JewelryModelRepository _modelRepository;
        private readonly JewelryRepository _jewelryRepository;

        #region classes





        #endregion
        public DbTest(TestDbFixture fixture)
        {
            _fixture = fixture;
            _context = fixture._context;
            _sender = new Mock<ISender>();
            _unitOfWork = new UnitOfWork(_context);
            _categoryRepository = new JewelryModelCategoryRepository(_context);
            _modelRepository = new JewelryModelRepository(_context, null);
            _jewelryRepository = new JewelryRepository(_context, null);
        }
        public static List<T> GetTestData<T>(string folder, string file) where T : class
        {
            var jsonData = File.ReadAllText(Path.Combine("Data", folder, $"{file}.json"));
            var data = JsonConvert.DeserializeObject(jsonData, typeof(List<T>)) as List<T>;
            return data;
        }

        public static IEnumerable<object[]> GetCategory(string folder, string file, int skip, int take)
        {
            var result = GetTestData<CategoryTest>(folder, file).Skip(skip).Take(take);
            foreach (var item in result)
            {
                yield return new object[] { item.name, item.description, item.isGeneral, item.parentCategoryId };
            }
        }
        [Trait("Database", "Size And Metal")]
        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenMetalAndSizeAddToDb()
        {
            List<Metal> METALS = new List<Metal>
            {
                Metal.Create("Platinum", 778_370, MetalId.Parse(1.ToString())),
                Metal.Create("14K Yellow Gold", 1_217_096, MetalId.Parse(2.ToString())),
                Metal.Create("14K White Gold", 1_217_096, MetalId.Parse(3.ToString())),
                Metal.Create("14K Pink Gold", 1_217_096, MetalId.Parse(4.ToString())),
                Metal.Create("16K Yellow Gold", 1_391_318, MetalId.Parse(5.ToString())),
                Metal.Create("16K White Gold", 1_391_318, MetalId.Parse(6.ToString())),
                Metal.Create("16K Pink Gold", 1_391_318, MetalId.Parse(7.ToString())),
                Metal.Create("18K Yellow Gold", 1_565_233, MetalId.Parse(8.ToString())),
                Metal.Create("18K White Gold", 1_565_233, MetalId.Parse(9.ToString())),
                Metal.Create("18K Pink Gold", 1_565_233, MetalId.Parse(10.ToString())),
            };
            List<Size> SIZES =
            Enumerable.Range(SizeRules.MinRingSize, SizeRules.MaxRingSize)
            .Select(p => Size.Create(p, null, SizeId.Parse(p.ToString()))).ToList();

            await _context.Metals.AddRangeAsync(METALS);
            await _context.Sizes.AddRangeAsync(SIZES);
        }


        [Trait("Database", "JewelryModelCategory")]
        [Theory]
        [MemberData(nameof(GetCategory), "JewelryModel", "InputCategory", 0, 1)]
        public async Task Handle_Should_ReturnSuccess_WhenCategoryAddToDb(string name, string desc, bool isGeneral, string? parentId)
        {
            var command = new CreateJewelryCategoryCommand(name, desc, isGeneral, parentId);
            var handler = new CreateJewelryCategoryCommandHandler(_categoryRepository, _unitOfWork);
            var result = await handler.Handle(command, default);
            result.IsSuccess.Should().BeTrue();
            result.Value
                .Should().NotBeNull()
                .And.BeOfType<JewelryModelCategory>();
        }

        public static IEnumerable<object[]> GetModel(string folder, string file, int skip, int take)
        {
            var result = GetTestData<ModelTest>(folder, file).Skip(skip).Take(take);
            foreach (var item in result)
            {
                yield return new object[] { item.modelSpec, item.mainDiamondSpecs, item.sideDiamondSpecs, item.metalSizeSpecs };
            }
        }

        [Trait("Database", "JewelryModel")]
        [Theory]
        [MemberData(nameof(GetModel), "JewelryModel", "InputModel", 1, 1)]
        public async Task Handle_Should_ReturnSuccess_WhenModelAddToDb(Modelspec ModelSpec, Maindiamondspec[]? mainDiamondSpecs, Sidediamondspec[]? sideDiamondSpecs, Metalsizespec[] metalSizeSpecs)
        {
            var categoryId = _context.JewelryModelCategories.FirstOrDefault()?.Id;
            categoryId.Should().NotBeNull();

            JewelryModelRequestDto modelReq = new(ModelSpec.name, categoryId.Value, ModelSpec.width, ModelSpec.length, ModelSpec.isEngravable, ModelSpec.isRhodiumFinish, ModelSpec.backType, ModelSpec.claspType, ModelSpec.chainType);
            List<MainDiamondRequestDto> mainReqs = mainDiamondSpecs is null ? null : mainDiamondSpecs.Select(p => new MainDiamondRequestDto(p.settingType, p.quantity, p.shapeSpecs.Select(s => new MainDiamondShapeRequestDto(s.shapeId, s.caratFrom, s.caratTo)).ToList())).ToList();
            List<SideDiamondRequestDto> sideReqs = sideDiamondSpecs is null ? null : sideDiamondSpecs.Select(p => new SideDiamondRequestDto(p.shapeId, p.colorMin, p.colorMax, p.clarityMin, p.clarityMax, p.settingType, p.optSpecs.Select(s => new SideDiamondOptRequestDto(s.caratWeight, s.quantity)).ToList())).ToList();
            List<ModelMetalSizeRequestDto> metalSizeReqs = metalSizeSpecs.Select(p => new ModelMetalSizeRequestDto(p.metalId, p.sizeId, p.weight)).ToList();

            var modelValidator = new CreateJewelryModelCommandValidator();
            var sizeMetalValidator = new CreateSizeMetalCommandValidator();
            var mainDiamondValidator = new CreateMainDiamondCommandValidator();
            var sideDiamondValidator = new CreateSideDiamondCommandValidator();

            var modelCommand = new CreateJewelryModelCommand(modelReq, mainReqs, sideReqs, metalSizeReqs);
            var handler = new CreateJewelryModelCommandHandler(_sender.Object,
                new JewelryModelCategoryRepository(_context),
                new JewelryModelRepository(_context, null), _unitOfWork);
            var sizeMetalHandler = new CreateSizeMetalCommandHandler(new SizeMetalRepository(_context, null), _unitOfWork);
            var mainDiamondHandler = new CreateMainDiamondCommandHandler(new MainDiamondRepository(_context, null), _unitOfWork);
            var sideDiamondHandler = new CreateSideDiamondCommandHandler(new SideDiamondRepository(_context), _unitOfWork);

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

            var added = _fixture._context.JewelryModels.AsQueryable();
            added.ToList().Should().ContainSingle(p => p.Id == result.Value.Id);
        }
        public static IEnumerable<object[]> GetJewelry(string folder, string file, int skip, int take)
        {
            var result = GetTestData<JewelTest>(folder, file).Skip(skip).Take(take);
            foreach (var item in result)
            {
                yield return new object[] { item.jewelryRequest, item.sideDiamondOptIds, item.attachedDiamondIds };
            }
        }

        [Trait("Database", "Jewelry")]
        [Theory]
        [MemberData(nameof(GetJewelry), "Jewelry", "InputJewelry", 0, 1)]
        public async Task Handle_Should_ReturnSuccess_WhenJewelryAddToDb(Jewelryrequest jewelryRequest, string[]? sideDiamondOptIds, string[]? attachedDiamondIds)
        {


            var model = await _context.JewelryModels.Include(p => p.SideDiamonds).FirstOrDefaultAsync(p => p.Name == "Test_Ring_No_Diamond");
            var sizeMetals = _context.SizeMetals.Where(p => p.ModelId == model.Id).ToList();

            var sideDiamonds = model.SideDiamonds;
            List<string> sideOpts = new();
            foreach (var side in sideDiamonds)
            {
                var opt = await _context.SideDiamondOpts.FirstOrDefaultAsync(p => p.SideDiamondReqId == side.Id);
                sideOpts.Add(opt.Id.Value);
            }
            if (sideOpts.Count == 0) sideOpts = null;
            List<string> attachDiamonds = new();
            if (attachDiamonds.Count == 0) attachDiamonds = null;
            var modelId = model.Id;
            modelId.Should().NotBeNull();

            var compareDiamondShapeValidator = new CompareDiamondShapeCommandValidator();
            var attachDiamondValidator = new AttachDiamondCommandValidator();
            var createJewelrySideDiamondValidator = new CreateJewelrySideDiamondCommandValidator();

            JewelryRequestDto jewelryReq = new JewelryRequestDto(modelId.Value, jewelryRequest.sizeId, jewelryRequest.metalId, jewelryRequest.serialCode);

            var jewelryCommand = new CreateJewelryCommand(jewelryReq, sideOpts, attachDiamonds);

            var handler = new CreateJewelryCommandHandler(
                new JewelryRepository(_context, null),
                new JewelryModelRepository(_context, null),
                new SizeMetalRepository(_context, null),
                new DiamondRepository(_context),
                _sender.Object,
                _unitOfWork,
                null
                );
            var compareDiamondShapeHandler = new CompareDiamondShapeCommandHandler(new MainDiamondRepository(_context, null));
            var attachDiamondHandler = new AttachDiamondCommandHandler(new DiamondRepository(_context), _unitOfWork);
            var createJewelrySideDiamondHandler = new CreateJewelrySideDiamondCommandHandler(new JewelrySideDiamondRepository(_context), new SideDiamondRepository(_context), _unitOfWork);

            _sender.Setup(s => s.Send(It.IsAny<CompareDiamondShapeCommand>(), It.IsAny<CancellationToken>()))
                .Returns(async (CompareDiamondShapeCommand command, CancellationToken token) =>
                {
                    var validate = compareDiamondShapeValidator.Validate(command);
                    if (validate.IsValid)
                        return await compareDiamondShapeHandler.Handle(command, token);
                    else
                        throw new ValidationException(validate.ToString());
                });

            _sender.Setup(s => s.Send(It.IsAny<AttachDiamondCommand>(), It.IsAny<CancellationToken>()))
                .Returns(async (AttachDiamondCommand command, CancellationToken token) =>
                {
                    var validate = attachDiamondValidator.Validate(command);
                    if (validate.IsValid)
                        return await attachDiamondHandler.Handle(command, token);
                    else
                        throw new ValidationException(validate.ToString());
                });
            _sender.Setup(s => s.Send(It.IsAny<CreateJewelrySideDiamondCommand>(), It.IsAny<CancellationToken>()))
                .Returns(async (CreateJewelrySideDiamondCommand command, CancellationToken token) =>
                {
                    var validate = createJewelrySideDiamondValidator.Validate(command);
                    if (validate.IsValid)
                        return await createJewelrySideDiamondHandler.Handle(command, token);
                    else
                        throw new ValidationException(validate.ToString());
                });
            var result = await handler.Handle(jewelryCommand, default);
            result.IsSuccess.Should().BeTrue();
            var added = _context.JewelryModels.AsQueryable();
            added.Should().HaveCount(1);
        }
    }
}

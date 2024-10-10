using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.JewelryModelCategories.Commands.Create;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Application.Usecases.MainDiamonds.Commands.Create;
using DiamondShop.Application.Usecases.SideDiamonds.Commands;
using DiamondShop.Application.Usecases.SizeMetals.Commands.Create;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryRepo;
using DiamondShop.Test.Application.JewelryModels.Create;
using DiamondShop.Test.General;
using FluentAssertions;
using MediatR;
using Moq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DiamondShop.Test.Application
{
    [Collection("Database collection")]
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
        public static IEnumerable<object[]> GetTestData<T>(string path = "", int skip = 0, int take = 9999)
        {
            var jsonData = File.ReadAllText($"Data/{path}.json");
            var data = JsonConvert.DeserializeObject<List<T>>(jsonData);
            foreach (var row in data.Skip(skip).Take(take))
            {
                var prop = typeof(T).GetProperties();
                yield return prop.Select(p => p.GetValue(row)).ToArray();
            }
        }

        public static IEnumerable<object[]> GetCategory(string path, int skip, int take) => GetTestData<CategoryTest>(path, skip, take);
        public static IEnumerable<object[]> GetModel(string path, int skip, int take) => GetTestData<ModelTest>(path, skip, take);

        [Trait("Database", "JewelryModelCategory")]
        [Theory]
        [MemberData(nameof(GetTestData), "JewelryModel/InputCategory", 1, 1)]
        public async Task Handle_Should_ReturnSuccess_WhenCategoryAddToDb(string name, string desc, bool isGeneral, string parentId)
        {
            var command = new CreateJewelryCategoryCommand(name, desc, isGeneral, parentId);
            var handler = new CreateJewelryCategoryCommandHandler(_categoryRepository, _unitOfWork);
            var result = await handler.Handle(command, default);
            result.IsSuccess.Should().BeTrue();
            result.Value
                .Should().NotBeNull()
                .And.BeOfType<JewelryModelCategory>();
        }


        [Trait("Database", "JewelryModel")]
        [Theory]
        [MemberData(nameof(GetTestData), "JewelryModel/InputModel", 1, 1)]
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
            var mainDiamondHandler = new CreateMainDiamondCommandHandler(new MainDiamondRepository(_context), _unitOfWork);
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
    }
    [CollectionDefinition("Database Collection")]
    public class DbCollection : ICollectionFixture<TestDbFixture> { }
}

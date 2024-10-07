using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.JewelryModelCategories.Commands.Create;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo;
using FluentAssertions;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.General.JewelryModels.Categories.Create
{
    public class CreateModelCategoryTest
    {

        public class JewelryModelCategoryJSON
        {
            public string name { get; set; }
            public string description { get; set; }
            public bool isGeneral { get; set; }
            public string parentCategoryId { get; set; }
        }

        private readonly Mock<IJewelryModelCategoryRepository> _categoryRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        public CreateModelCategoryTest()
        {
            _categoryRepo = new Mock<IJewelryModelCategoryRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
        }
        [Fact]
        public async Task Handle_Should_CallCreateFunc()
        {
            var command = new CreateJewelryCategoryCommand("Test_Category", "this is a testing category, don't use", true, null);
            _categoryRepo.Setup(
             x => x.CheckDuplicate(
                 It.IsAny<string>()))
             .ReturnsAsync(false);
            var handler = new CreateJewelryCategoryCommandHandler(_categoryRepo.Object, _unitOfWork.Object);

            var result = await handler.Handle(command, default);

            _categoryRepo.Verify(x => x.Create(It.Is<JewelryModelCategory>(p => p.Id == result.Value.Id), default), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenNameIsNotUnique()
        {
            var command = new CreateJewelryCategoryCommand("Test_Category", "this is a testing category, don't use", true, null);

            var handler = new CreateJewelryCategoryCommandHandler(_categoryRepo.Object, _unitOfWork.Object);


            var result = await handler.Handle(command, default);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle()
                .Which.Message.Should().Be("This category name has already been used");
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResult_WhenNameIsNotUnique()
        {
            var command = new CreateJewelryCategoryCommand("Test_Category", "this is a testing category, don't use", true, null);
            
            _categoryRepo.Setup(
                x => x.CheckDuplicate(
                    It.IsAny<string>()))
                .ReturnsAsync(false);

            var handler = new CreateJewelryCategoryCommandHandler(_categoryRepo.Object, _unitOfWork.Object);


            var result = await handler.Handle(command, default);

            result.IsSuccess.Should().BeTrue();
            result.Value
                .Should().NotBeNull()
                .And.BeOfType<JewelryModelCategory>();
        }
        public static IEnumerable<object[]> GetTestData()
        {
            var jsonData = File.ReadAllText("Data/InputCategory.json");
            var data = JsonConvert.DeserializeObject<List<JewelryModelCategoryJSON>>(jsonData);
            foreach(var row in data)
            {
                yield return new object[] { row.name, row.description, row.isGeneral, row.parentCategoryId };
            }
        }
        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Handle_Should_ReturnSuccess_WhenCategoryAddToDb(string name, string desc, bool isGeneral, string parentId)
        {
            
            DbContextOptions opt = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase($"CategoryTest {new Guid().ToString()}")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            using (var context = new TestDbContext(opt))
            {
                var unitOfWork = new UnitOfWork(context);
                var impl = new JewelryModelCategoryRepository(context);
                var command = new CreateJewelryCategoryCommand(name, desc, isGeneral, parentId);
                var handler = new CreateJewelryCategoryCommandHandler(impl, unitOfWork);


                var result = await handler.Handle(command, default);
                result.IsSuccess.Should().BeTrue();
                result.Value
                    .Should().NotBeNull()
                    .And.BeOfType<JewelryModelCategory>();
            }
        }

    }
}

using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.JewelryModelCategories.Commands.Create;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentAssertions;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.General.JewelryModels.Categories.Create
{
    public class CreateModelCategoryTest
    {
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

            _categoryRepo.Verify(x => x.Create(It.Is<JewelryModelCategory>(p => p.Id == result.Value.Id), default),Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenNameIsNotUnique()
        {
            var command = new CreateJewelryCategoryCommand("Test_Category", "this is a testing category, don't use", true, null);
            _categoryRepo.Setup(
                x => x.CheckDuplicate(
                    It.IsAny<string>()))
                .ReturnsAsync(true);

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
    }
}

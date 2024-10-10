using DiamondShop.Api.Controllers.JewelryModels;
using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.SizeMetals.Commands.Create;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Infrastructure.Databases;
using FluentAssertions;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.Application.JewelryModels.Create
{
    public class SizeMetalHolder()
    {
        public string modelId { get; set; }
        public List<ModelMetalSizeRequestDto> metalSizeSpecs { get; set; }
        public bool expected { get; set; }
    }
    [Trait(nameof(JewelryModels), "Size Metal")]
    public class CreateSizeMetalTest
    {
        private readonly Mock<ISizeMetalRepository> _sizeMetalRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        public CreateSizeMetalTest()
        {
            _sizeMetalRepo = new Mock<ISizeMetalRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
        }
        public static IEnumerable<object[]> GetTestData()
        {
            var jsonData = File.ReadAllText("Data/JewelryModel/InputSizeMetal.json");
            var data = JsonConvert.DeserializeObject<List<SizeMetalHolder>>(jsonData);
            foreach (var item in data)
            {
                yield return new object[] { item.modelId, item.metalSizeSpecs, item.expected };
            }
        }
        [Theory]
        [MemberData(nameof(GetTestData))]
        public void Handle_Should_ReturnFailure_WhenListHasDuplicant(string modelId, List<ModelMetalSizeRequestDto> metalSizeSpecs, bool expected)
        {
            var command = new CreateSizeMetalCommand(JewelryModelId.Parse(modelId), metalSizeSpecs);
            var validator = new CreateSizeMetalCommandValidator();

            var result = validator.Validate(command);

            if (expected)
            {
                result.IsValid.Should().BeTrue();
            }
            else
            {
                result.IsValid.Should().BeFalse();
                result.Errors.Should().ContainSingle(e => e.PropertyName == "MetalSizeSpecs" && e.ErrorMessage == "No duplicates allowed");
            }
        }
    }
}

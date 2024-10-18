using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.SideDiamonds.Commands;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.Application.JewelryModels.Create
{
    [Trait(nameof(JewelryModels), "Side Diamond")]
    public class CreateSideDiamondTest
    {
        private readonly Mock<ISideDiamondRepository> _sideDiamondRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        public CreateSideDiamondTest()
        {
            _sideDiamondRepo = new Mock<ISideDiamondRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
        }
        [Fact]
        public void Handle_Should_ReturnFailure_WhenColorMinIsGreaterThanColorMax()
        {
            List<SideDiamondOptRequestDto> opts = new()
                {
                    new SideDiamondOptRequestDto(2.4f,2),
                };
            //Sua lai sau khi doi cho Color
            SideDiamondRequestDto spec = new("1", Color.D, Color.K, Clarity.S11, Clarity.S11, SettingType.Prong, opts);
            var command = new CreateSideDiamondCommand(JewelryModelId.Create(), spec);
            var validator = new CreateSideDiamondCommandValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "SideDiamondSpecs.ColorMax");
        }

        [Fact]
        public void Handle_Should_ReturnSuccess_WhenColorMinIsSmallerThanColorMax()
        {
            List<SideDiamondOptRequestDto> opts = new()
                {
                    new SideDiamondOptRequestDto(2.4f,2),
                };
            SideDiamondRequestDto spec = new("1", Color.K, Color.D, Clarity.S11, Clarity.S11, SettingType.Prong, opts);
            var command = new CreateSideDiamondCommand(JewelryModelId.Create(), spec);
            var validator = new CreateSideDiamondCommandValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}

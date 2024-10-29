using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Usecases.Jewelries.Commands;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Test.Integration.Data;
using FluentResults;
using Xunit.Abstractions;

namespace DiamondShop.Test.Integration
{
    public class JewelryTest : BaseIntegrationTest
    {
        protected readonly ITestOutputHelper _output;
        public JewelryTest(IntegrationWAF factory, ITestOutputHelper output) : base(factory)
        {
            _output = output;
        }


        [Trait("ReturnTrue", "DefaultRing")]
        [Fact]
        public async Task Create_DefaultRing_Should_AddToDb()
        {
            var model = await TestData.SeedDefaultRingModel(_context);
            var diamond = await TestData.SeedDefaultDiamond(_context);

            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, "Default_Ring_1", Domain.Common.Enums.ProductStatus.Active);
            var sideDiamondOptions = model.SideDiamonds.Select(p => p.SideDiamondOpts.First().Id.Value).ToList();
            var attachedDiamonds = new List<string>() { diamond.Id.Value };
            var command = new CreateJewelryCommand(jewelryReq, sideDiamondOptions, attachedDiamonds);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                _output.WriteLine(result.Errors[0].Message);
            }
            Assert.True(result.IsSuccess);
        }
        [Trait("ReturnTrue", "MultiMainDiamondRing")]
        [Fact]
        public async Task Create_MultiMainDiamondRing_Should_AddToDb()
        {
            var model = await TestData.SeedMultiMainDiamondRingModel(_context);
            var diamonds = await TestData.SeedDefaultDiamonds(_context, 3, "1");

            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, "MultiMain_Ring_1", Domain.Common.Enums.ProductStatus.Active);
            var sideDiamondOptions = model.SideDiamonds.Select(p => p.SideDiamondOpts.First().Id.Value).ToList();
            var attachedDiamonds = diamonds.Select(p => p.Id.Value).ToList();
            var command = new CreateJewelryCommand(jewelryReq, sideDiamondOptions, attachedDiamonds);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                _output.WriteLine(result.Errors[0].Message);
            }
            Assert.True(result.IsSuccess);
        }
        [Trait("ReturnTrue", "NoDiamondRing")]
        [Fact]
        public async Task Create_NoDiamondRing_Should_AddToDb()
        {
            var model = await TestData.SeedNoDiamondRingModel(_context);

            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, "NoDiamond_Ring_1", Domain.Common.Enums.ProductStatus.Active);
            var command = new CreateJewelryCommand(jewelryReq, null, null);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                _output.WriteLine(result.Errors[0].Message);
            }
            Assert.True(result.IsSuccess);
        }
        [Trait("ReturnTrue", "DefaultNecklace")]
        [Fact]
        public async Task Create_DefaultNecklace_Should_AddToDb()
        {
            var model = await TestData.SeedDefaultNecklaceModel(_context);
            var diamond = await TestData.SeedDefaultDiamond(_context);

            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, "Default_Ring_1", Domain.Common.Enums.ProductStatus.Active);
            var sideDiamondOptions = model.SideDiamonds.Select(p => p.SideDiamondOpts.First().Id.Value).ToList();
            var attachedDiamonds = new List<string>() { diamond.Id.Value };
            var command = new CreateJewelryCommand(jewelryReq, sideDiamondOptions, attachedDiamonds);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                _output.WriteLine(result.Errors[0].Message);
            }
            Assert.True(result.IsSuccess);
        }
        [Trait("ReturnFalse", "UnmatchingMainDiamondCount")]
        [Fact]
        public async Task Create_DefaultRing_WithDifferentDiamondCount_ShouldNot_AddToDb()
        {
            var model = await TestData.SeedMultiMainDiamondRingModel(_context);
            var diamonds = await TestData.SeedDefaultDiamonds(_context, 2, "1");

            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, "MultiMain_Ring_1", Domain.Common.Enums.ProductStatus.Active);
            var sideDiamondOptions = model.SideDiamonds.Select(p => p.SideDiamondOpts.First().Id.Value).ToList();
            var attachedDiamonds = diamonds.Select(p => p.Id.Value).ToList();
            var command = new CreateJewelryCommand(jewelryReq, sideDiamondOptions, attachedDiamonds);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                _output.WriteLine(result.Errors[0].Message);
            }
            Assert.True(result.IsFailed);
        }
        [Trait("ReturnFalse", "UnmatchingMainDiamondShape")]
        [Fact]
        public async Task Create_DefaultRing_WithDifferentDiamondShape_ShouldNot_AddToDb()
        {
            var model = await TestData.SeedMultiMainDiamondRingModel(_context);
            var diamonds = await TestData.SeedDefaultDiamonds(_context, 3, "4");

            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, "MultiMain_Ring_1", Domain.Common.Enums.ProductStatus.Active);
            var sideDiamondOptions = model.SideDiamonds.Select(p => p.SideDiamondOpts.First().Id.Value).ToList();
            var attachedDiamonds = diamonds.Select(p => p.Id.Value).ToList();
            var command = new CreateJewelryCommand(jewelryReq, sideDiamondOptions, attachedDiamonds);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                _output.WriteLine(result.Errors[0].Message);
            }
            Assert.True(result.IsFailed);
        }

    }
}

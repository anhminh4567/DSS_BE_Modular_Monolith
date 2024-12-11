using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Usecases.Jewelries.Commands.Create;
using DiamondShop.Application.Usecases.Jewelries.Commands.Delete;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Test.Integration.Data;
using FluentResults;
using Microsoft.EntityFrameworkCore;
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
        [Trait("ReturnTrue", "DeleteJewelry")]
        [Fact]
        public async Task Delete_Jewelry_Should_Detach_Diamond()
        {
            var jewelry = await TestData.SeedDefaultJewelry(_context);
            var diamond = await TestData.SeedDefaultDiamond(_context, jewelry.Id);
            Assert.NotNull(diamond.JewelryId);
            _context.Set<Jewelry>().Remove(jewelry);
            await _context.SaveChangesAsync();
            var jewelries = _context.Set<Jewelry>().ToList();
            Assert.Equal(0, jewelries.Count);
            Assert.Null(diamond.JewelryId);
        }

        [Trait("ReturnTrue", "DefaultRing")]
        [Fact]
        public async Task Create_DefaultRing_Should_AddToDb()
        {
            var model = await TestData.SeedDefaultRingModel(_context);
            var diamond = await TestData.SeedDefaultDiamond(_context);

            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, null, Domain.Common.Enums.ProductStatus.Active);
            var attachedDiamonds = new List<string>() { diamond.Id.Value };
            var command = new CreateJewelryCommand(jewelryReq, model.SideDiamonds.FirstOrDefault().Id.Value, attachedDiamonds);
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

            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, null, Domain.Common.Enums.ProductStatus.Active);
            var attachedDiamonds = diamonds.Select(p => p.Id.Value).ToList();
            var command = new CreateJewelryCommand(jewelryReq, model.SideDiamonds.FirstOrDefault().Id.Value, attachedDiamonds);
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

            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, null, Domain.Common.Enums.ProductStatus.Active);
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

            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, null, Domain.Common.Enums.ProductStatus.Active);
            var attachedDiamonds = new List<string>() { diamond.Id.Value };
            var command = new CreateJewelryCommand(jewelryReq, model.SideDiamonds.FirstOrDefault().Id.Value, attachedDiamonds);
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
            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, null, Domain.Common.Enums.ProductStatus.Active);
            var command = new CreateJewelryCommand(jewelryReq, model.SideDiamonds.FirstOrDefault().Id.Value, null);
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

            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, null, Domain.Common.Enums.ProductStatus.Active);
            var attachedDiamonds = diamonds.Select(p => p.Id.Value).ToList();
            var command = new CreateJewelryCommand(jewelryReq, model.SideDiamonds.FirstOrDefault().Id.Value, attachedDiamonds);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                _output.WriteLine(result.Errors[0].Message);
            }
            Assert.True(result.IsFailed);
        }
        [Trait("ReturnTrue", "DeleteJewelryStillCreateNewOne")]
        [Fact]
        public async Task Delete_Jewelry_Should_StillAbleTo_Create_New_One_AddToDb()
        {
            var model = await TestData.SeedNoDiamondRingModel(_context);
            var jewelryReq = new JewelryRequestDto(model.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, null, Domain.Common.Enums.ProductStatus.Active);
            for (int i = 0; i < 3; i++)
            {
                var createResult = await _sender.Send(new CreateJewelryCommand(jewelryReq, null, null));
                if (createResult.IsFailed)
                {
                    _output.WriteLine(createResult.Errors[0].Message);
                }
                Assert.True(createResult.IsSuccess);
            }
            var jewelryBefore = await _context.Set<Jewelry>().ToListAsync();
            foreach (var jewelry in jewelryBefore)
                _output.WriteLine(jewelry.SerialCode);
            var deleteResult = await _sender.Send(new DeleteJewelryCommand(jewelryBefore[1].Id.Value));
            if (deleteResult.IsFailed)
            {
                _output.WriteLine(deleteResult.Errors[0].Message);
            }
            Assert.True(deleteResult.IsSuccess);
            var jewelryWhile = await _context.Set<Jewelry>().ToListAsync();
            _output.WriteLine("");
            foreach (var jewelry in jewelryWhile)
                _output.WriteLine(jewelry.SerialCode);
            var command = new CreateJewelryCommand(jewelryReq, null, null);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                _output.WriteLine(result.Errors[0].Message);
            }
            Assert.True(result.IsSuccess);
            _output.WriteLine("");
            var jewelryAfter = await _context.Set<Jewelry>().ToListAsync();
            foreach (var jewelry in jewelryAfter)
                _output.WriteLine(jewelry.SerialCode);
        }
    }
}

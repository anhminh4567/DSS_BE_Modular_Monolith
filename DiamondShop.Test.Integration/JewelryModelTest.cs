using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Test.Integration.Data;
using Xunit.Abstractions;

namespace DiamondShop.Test.Integration
{
    public class JewelryModelTest : BaseIntegrationTest
    {
        protected readonly ITestOutputHelper _output;
        public JewelryModelTest(IntegrationWAF factory, ITestOutputHelper output) : base(factory)
        {
            _output = output;
        }

        [Trait("ReturnTrue", "DefaultRing")]
        [Fact]
        public async Task Create_OneMain_OneSide_RingModel_Should_AddToDb()
        {

            var modelSpec = new JewelryModelRequestDto("Test_Model", "TM", TestData.DefaultCategoryId.Value, 0, 1f, null, false, false, null, null, null);
            var mainDiamondSpec =
                new List<MainDiamondRequestDto>()
                {
                    new MainDiamondRequestDto(SettingType.Prong, 1, new()
                    {
                        new ("1",0.3f,2.5f),
                        new ("2",0.3f,2.5f),
                        new ("3",0.3f,2.5f),
                    })
                };

            var sideDiamondSpec = new List<SideDiamondRequestDto>()
            {
                new ("1",Color.K, Color.D,Clarity.VS2,Clarity.VS1,SettingType.Prong, 0.05f, 5)
            };

            var metalSizeSpec = new List<ModelMetalSizeRequestDto>()
            {
                new (MetalId: "2", SizeId: "3", 10),
                new (MetalId: "2", SizeId: "4", 12),
                new (MetalId: "2", SizeId: "5", 14),
                new (MetalId: "2", SizeId: "6", 16),
            };
            var command = new CreateJewelryModelCommand(modelSpec, mainDiamondSpec, sideDiamondSpec, metalSizeSpec);
            var result = await _sender.Send(command);
            Assert.True(result.IsSuccess);
        }

        [Trait("ReturnTrue", "NoDiamond")]
        [Fact]
        public async Task Create_NoDiamond_RingModel_Should_AddToDb()
        {

            var modelSpec = new JewelryModelRequestDto("Test_Model", "TM", TestData.DefaultCategoryId.Value, 0, 1f, null, false, false, null, null, null);
            var metalSizeSpec = new List<ModelMetalSizeRequestDto>()
            {
                new (MetalId: "2", SizeId: "3", 10),
                new (MetalId: "2", SizeId: "4", 12),
                new (MetalId: "2", SizeId: "5", 14),
                new (MetalId: "2", SizeId: "6", 16),
            };
            var command = new CreateJewelryModelCommand(modelSpec, null, null, metalSizeSpec);
            var result = await _sender.Send(command);
            Assert.True(result.IsSuccess);
        }

        [Trait("ReturnTrue", "DefaultNonRing")]
        [Fact]
        public async Task Create_OneMain_OneSide_NecklaceModel_Should_AddToDb()
        {
            var modelSpec = new JewelryModelRequestDto("Test_Model", "TM",TestData.DefaultCategoryId.Value,0, null, null, false, false, null, ClaspType.Open_Box, ChainType.Rope);
            var mainDiamondSpec =
                new List<MainDiamondRequestDto>()
                {
                    new MainDiamondRequestDto(SettingType.Prong, 1, new()
                    {
                        new ("1",0.3f,2.5f),
                        new ("2",0.3f,2.5f),
                        new ("3",0.3f,2.5f),
                    })
                };

            var sideDiamondSpec = new List<SideDiamondRequestDto>()
            {
                new ("1",Color.K, Color.D,Clarity.VS2,Clarity.VS1,SettingType.Prong, 0.05f, 5)
            };

            var metalSizeSpec = new List<ModelMetalSizeRequestDto>()
            {
                new (MetalId: "2", SizeId: "3", 10),
                new (MetalId: "2", SizeId: "4", 12),
                new (MetalId: "2", SizeId: "5", 14),
                new (MetalId: "2", SizeId: "6", 16),
            };
            var command = new CreateJewelryModelCommand(modelSpec, mainDiamondSpec, sideDiamondSpec, metalSizeSpec);
            var result = await _sender.Send(command);
            Assert.True(result.IsSuccess);
        }

        [Trait("ReturnTrue", "MultiMainSideRing")]
        [Fact]
        public async Task Create_MultiMain_MultiSide_RingModel_Should_AddToDb()
        {

            var modelSpec = new JewelryModelRequestDto("Test_Model", "TM", TestData.DefaultCategoryId.Value, 0, 1f, null, false, false, null, null, null);
            var mainDiamondSpec =
                new List<MainDiamondRequestDto>()
                {
                    new MainDiamondRequestDto(SettingType.Prong, 2, new()
                    {
                        new ("1",0.3f,2.5f),
                        new ("2",0.3f,2.5f),
                        new ("3",0.3f,2.5f),
                    }),
                    new MainDiamondRequestDto(SettingType.Prong, 1, new()
                    {
                        new ("1",0.3f,2.5f),
                        new ("2",0.3f,2.5f),
                        new ("3",0.3f,2.5f),
                    }),
                };

            var sideDiamondSpec = new List<SideDiamondRequestDto>()
            {
                new ("1",Color.K, Color.D,Clarity.VS2,Clarity.VS1,SettingType.Prong, 0.05f, 20),
                new ("1",Color.K, Color.D,Clarity.VS2,Clarity.VS1,SettingType.Prong, 0.25f, 15),
                new ("1",Color.K, Color.D,Clarity.VS2,Clarity.VS1,SettingType.Prong, 1f, 5),
            };

            var metalSizeSpec = new List<ModelMetalSizeRequestDto>()
            {
                new (MetalId: "2", SizeId: "3", 10),
                new (MetalId: "2", SizeId: "4", 12),
                new (MetalId: "2", SizeId: "5", 14),
                new (MetalId: "2", SizeId: "6", 16),
            };
            var command = new CreateJewelryModelCommand(modelSpec, mainDiamondSpec, sideDiamondSpec, metalSizeSpec);
            var result = await _sender.Send(command);
            Assert.True(result.IsSuccess);
        }

        [Trait("ReturnFalse", "DuplicateSideRing")]
        [Fact]
        public async Task Create_MultiSideWithDuplicateOpt_RingModel_ShouldNot_AddToDb()
        {

            var modelSpec = new JewelryModelRequestDto("Test_Model", "TM", TestData.DefaultCategoryId.Value, 0, 1f, null, false, false, null, null, null);
            var sideDiamondSpec = new List<SideDiamondRequestDto>()
            {
                      new ("1",Color.K, Color.D,Clarity.VS2,Clarity.VS1,SettingType.Prong, 0.05f, 20),
                new ("1",Color.K, Color.D,Clarity.VS2,Clarity.VS1,SettingType.Prong, 0.25f, 15),
            };

            var metalSizeSpec = new List<ModelMetalSizeRequestDto>()
            {
                new (MetalId: "2", SizeId: "3", 10),
                new (MetalId: "2", SizeId: "4", 12),
                new (MetalId: "2", SizeId: "5", 14),
                new (MetalId: "2", SizeId: "6", 16),
            };
            var command = new CreateJewelryModelCommand(modelSpec, null, sideDiamondSpec, metalSizeSpec);
            var result = await _sender.Send(command);
            Assert.True(result.IsFailed);
            result.Errors.ForEach(p => _output.WriteLine($"{p.Message}"));
        }

        [Trait("ReturnFalse", "DuplicateSizeMetalRing")]
        [Fact]
        public async Task Create_DuplicateSizeMetal_RingModel_ShouldNot_AddToDb()
        {

            var modelSpec = new JewelryModelRequestDto("Test_Model", "TM", TestData.DefaultCategoryId.Value, 0, 1f, null, false, false, null, null, null);
            var metalSizeSpec = new List<ModelMetalSizeRequestDto>()
            {
                new (MetalId: "2", SizeId: "3", 10),
                new (MetalId: "2", SizeId: "3", 12),
                new (MetalId: "2", SizeId: "5", 14),
                new (MetalId: "2", SizeId: "6", 16),
            };
            var command = new CreateJewelryModelCommand(modelSpec, null, null, metalSizeSpec);
            var result = await _sender.Send(command);
            Assert.True(result.IsFailed);
            result.Errors.ForEach(p => _output.WriteLine($"{p.Message}"));
        }
    }
}

using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Usecases.Jewelries.Commands;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Test.Integration.Data;

namespace DiamondShop.Test.Integration
{
    public class JewelryTest : BaseIntegrationTest
    {
        public JewelryTest(IntegrationWAF factory) : base(factory)
        {
        }
        async Task SeedCategory()
        {
            _context.Set<JewelryModelCategory>().Add(TestData.DefaultCategory);
            await _context.SaveChangesAsync();
        }
        async Task SeedDefaultRingModel()
        {
            await SeedCategory();
            _context.Set<JewelryModel>().Add(TestData.DefaultRingModel);
            await _context.SaveChangesAsync();
        }
        async Task SeedNoDiamondRingModel() { }


        [Trait("ReturnTrue", "DefaultJewelryRing")]
        [Fact]
        public async Task Create_DefaultJewelry_Should_AddToDb()
        {
            await SeedDefaultRingModel();
            var jewelryReq = new JewelryRequestDto(TestData.DefaultRingModel.Id.Value, TestData.SizeIds[0].Value, TestData.MetalIds[0].Value, "Default_Ring_1",true,true);
            var sideDiamondOptions = TestData.DefaultRingSideDiamondOpts.Where(p => p.Id.Value == "1_1_1").Select(p => p.Id.Value).ToList();
            var attachedDiamonds = new List<string>() { TestData.DefaultDiamond.Id.Value };
            var command = new CreateJewelryCommand(jewelryReq,sideDiamondOptions, attachedDiamonds);
            var result = await _sender.Send(command);
            Assert.True(result.IsSuccess);
        }
    }
}

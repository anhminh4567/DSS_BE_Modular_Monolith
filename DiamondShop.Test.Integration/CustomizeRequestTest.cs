using DiamondShop.Application.Usecases.CustomizeRequests.Commands.SendRequest;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Test.Integration.Data;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace DiamondShop.Test.Integration
{
    public class CustomizeRequestTest : BaseIntegrationTest
    {
        protected readonly ITestOutputHelper _output;

        public CustomizeRequestTest(IntegrationWAF factory, ITestOutputHelper output) : base(factory)
        {
            _output = output;
        }
        void WriteError(List<IError> errors)
        {
            foreach (var error in errors)
                _output.WriteLine(error.Message);
        }
        //async Task SeedingSideDiamondPrice(SideDiamondOpt diamond)
        //{
        //    TestData.SeedDefaultDiamondCriteria(_context,null,null,null)
        //}

        async Task<CustomizeRequest> SeedingDiamondRequest(AccountId accountId, JewelryModel jewelryModel)
        {
            Assert.NotNull(jewelryModel.SizeMetals);
            var sizeMetal = jewelryModel.SizeMetals.FirstOrDefault();

            Assert.NotNull(jewelryModel.MainDiamonds);
            var mainDiamond = jewelryModel.MainDiamonds?.FirstOrDefault();

            Assert.NotNull(mainDiamond);
            var firstShape = mainDiamond.Shapes.FirstOrDefault();
            Assert.NotNull(firstShape);

            //model has 3 main diamond
            List<CustomizeDiamondRequest> diamondRequests = new()
            {
                new(firstShape.ShapeId.Value,Clarity.IF,Color.D,Cut.Good,firstShape.CaratFrom,firstShape.CaratTo,false,Polish.Excellent,Symmetry.Excellent,Girdle.Thin, Culet.Medium),
            };
            CustomizeModelRequest customizeModelRequest = new(jewelryModel.Id.Value, sizeMetal.MetalId.Value, sizeMetal.SizeId.Value, jewelryModel.SideDiamonds[0].Id.Value, null, null, null, diamondRequests);
            var result = await _sender.Send(new CreateCustomizeRequestCommand(accountId.Value, customizeModelRequest));
            if (result.IsFailed)
                WriteError(result.Errors);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            return result.Value;
        }
        async Task<CustomizeRequest> SeedingNoDiamondRequest(AccountId accountId, JewelryModel jewelryModel)
        {
            if (jewelryModel.SideDiamonds != null) Assert.Equal(0, jewelryModel.SideDiamonds.Count());
            if (jewelryModel.MainDiamonds != null) Assert.Equal(0, jewelryModel.MainDiamonds.Count());
            var sizeMetal = jewelryModel.SizeMetals.FirstOrDefault();
            //model has 3 main diamond
            CustomizeModelRequest customizeModelRequest = new(jewelryModel.Id.Value, sizeMetal.MetalId.Value, sizeMetal.SizeId.Value, null, null, null, null, null);
            var result = await _sender.Send(new CreateCustomizeRequestCommand(accountId.Value, customizeModelRequest));
            if (result.IsFailed)
                WriteError(result.Errors);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            return result.Value;
        }

        [Trait("ReturnTrue", "CreatePendingRequest")]
        [Fact]
        public async Task Create_UnpricedSideDiamond_OrDiamond_CustomizeRequest_Should_ReturnPending()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var model = await TestData.SeedDefaultRingModel(_context);
            var result = await SeedingDiamondRequest(account.Id, model);
            var request = await _context.Set<CustomizeRequest>().FirstOrDefaultAsync();
            _output.WriteLine(request.ToString());
            Assert.Equal(CustomizeRequestStatus.Pending, request.Status);
        }
        [Trait("ReturnTrue", "CreateRequestingRequest")]
        [Fact]
        public async Task Create_PricedDiamond_AndNoDiamond_CustomizeRequest_Should_ReturnRequesting()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var model = await TestData.SeedNoDiamondRingModel(_context);
            var result = await SeedingNoDiamondRequest(account.Id, model);
            var request = await _context.Set<CustomizeRequest>().FirstOrDefaultAsync();
            _output.WriteLine(request.ToString());
            Assert.Equal(CustomizeRequestStatus.Requesting, request.Status);
        }
        [Trait("ReturnTrue", "ProceedRequest_HappyCase")]
        [Fact]
        public async Task Proceed_CustomizeRequest_HappyCase_Should_ReturnSucess()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var model = await TestData.SeedDefaultRingModel(_context);
            //var result = await SeedingRequest(account.Id, model);

        }
    }
}

using DiamondShop.Application.Dtos.Requests.Diamonds;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Staff;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.Reject;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.SendRequest;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateMany;
using DiamondShop.Application.Usecases.DiamondPrices.Commands.CreateMany;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Jewelries;
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
        async Task SeedingSideDiamondPrice(SideDiamondOpt diamond)
        {
            List<DiamondCriteriaRequestDto> criteriaRequestDtos = new() {
                new()
                {
                    CaratFrom = 0.001f,
                    CaratTo = 3f,
                }
            };
            var result = await _sender.Send(new CreateManyDiamondCriteriasCommand(criteriaRequestDtos,true));
            if (result.IsFailed)
                WriteError(result.Errors);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            var criteria = result.Value[0];
            Assert.NotNull(criteria);
            List<DiamondPriceRequestDto> priceRequestDtos = new()
            {
                new(criteria.Id.Value,100000m),
            };
            var priceResult = await _sender.Send(new CreateManyDiamondPricesCommand(priceRequestDtos,DiamondShape.IsFancyShape(diamond.ShapeId),false,true));
            if (priceResult.IsFailed)
                WriteError(priceResult.Errors);
            Assert.True(priceResult.IsSuccess);
        }

        async Task<CustomizeRequest> SeedingDefaultModelRequest(AccountId accountId)
        {
            var jewelryModel = await TestData.SeedDefaultRingModel(_context);
            Assert.NotNull(jewelryModel.SizeMetals);
            var sizeMetal = jewelryModel.SizeMetals.FirstOrDefault();

            Assert.NotNull(jewelryModel.MainDiamonds);
            var mainDiamond = jewelryModel.MainDiamonds?.FirstOrDefault();

            Assert.NotNull(mainDiamond);
            var firstShape = mainDiamond.Shapes.FirstOrDefault();
            Assert.NotNull(firstShape);

            //model has 1 main diamond
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
        async Task<CustomizeRequest> SeedingNoDiamondRequest(AccountId accountId)
        {
            var jewelryModel = await TestData.SeedNoDiamondRingModel(_context);
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
        async Task<CustomizeRequest> SeedingPricedRequest(AccountId userId)
        {
            var jewelryModel = await TestData.SeedDefaultRingModel(_context);
            var diamond = await TestData.SeedDefaultDiamond(_context);
            var side = jewelryModel.SideDiamonds[0];
            Assert.NotNull(side);

            Assert.NotNull(jewelryModel.SizeMetals);
            var sizeMetal = jewelryModel.SizeMetals.FirstOrDefault();

            Assert.NotNull(jewelryModel.MainDiamonds);
            var mainDiamond = jewelryModel.MainDiamonds?.FirstOrDefault();

            Assert.NotNull(mainDiamond);
            var firstShape = mainDiamond.Shapes.FirstOrDefault();
            Assert.NotNull(firstShape);

            //model has 1 main diamond
            List<CustomizeDiamondRequest> diamondRequests = new()
            {
                new(diamond.DiamondShapeId.Value,diamond.Clarity,diamond.Color,diamond.Cut,firstShape.CaratFrom,firstShape.CaratTo,false,null,null,null,null),
            };
            CustomizeModelRequest customizeModelRequest = new(jewelryModel.Id.Value, sizeMetal.MetalId.Value, sizeMetal.SizeId.Value, side.Id.Value, null, null, null, diamondRequests);
            var result = await _sender.Send(new CreateCustomizeRequestCommand(userId.Value, customizeModelRequest));
            if (result.IsFailed)
                WriteError(result.Errors);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            var pendingRequest = await _context.Set<CustomizeRequest>().FirstOrDefaultAsync();
            _output.WriteLine(pendingRequest.ToString());
            Assert.Equal(CustomizeRequestStatus.Pending, pendingRequest.Status);

            await SeedingSideDiamondPrice(side);
            Assert.NotNull(pendingRequest.DiamondRequests);
            var diamondReq = pendingRequest.DiamondRequests[0];
            Assert.NotNull(diamondReq);

            DiamondRequestAssignRecord record = new(diamondReq.DiamondRequestId.Value, diamond.Id.Value);
            var pricedResult = await _sender.Send(new StaffProceedCustomizeRequestCommand(pendingRequest.Id.Value, new() { record }));
            if (pricedResult.IsFailed)
                WriteError(pricedResult.Errors);
            Assert.True(pricedResult.IsSuccess);
            Assert.NotNull(pricedResult.Value);
            return pricedResult.Value;
        }
        async Task<CustomizeRequest> SeedingAcceptedRequest(AccountId userId)
        {
            var requestingResult = await SeedingNoDiamondRequest(userId);
            var result = await _sender.Send(new StaffProceedCustomizeRequestCommand(requestingResult.Id.Value, null));
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
            var result = await SeedingDefaultModelRequest(account.Id);
            var request = await _context.Set<CustomizeRequest>().FirstOrDefaultAsync();
            _output.WriteLine(request.ToString());
            Assert.Equal(CustomizeRequestStatus.Pending, request.Status);
        }
        [Trait("ReturnTrue", "CreateRequestingRequest")]
        [Fact]
        public async Task Create_PricedSide_NoDiamond_CustomizeRequest_Should_ReturnRequesting()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var jewelryModel = await TestData.SeedSideDiamondOnlyRingModel(_context);
            Assert.NotNull(jewelryModel.SizeMetals);
            Assert.NotNull(jewelryModel.SideDiamonds);
            var side = jewelryModel.SideDiamonds[0];
            Assert.NotNull(side);
            await SeedingSideDiamondPrice(side);
            var sizeMetal = jewelryModel.SizeMetals.FirstOrDefault();
            CustomizeModelRequest customizeModelRequest = new(jewelryModel.Id.Value, sizeMetal.MetalId.Value, sizeMetal.SizeId.Value, side.Id.Value, null, null, null, null);
            var result = await _sender.Send(new CreateCustomizeRequestCommand(account.Id.Value, customizeModelRequest));
            if (result.IsFailed)
                WriteError(result.Errors);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            var request = await _context.Set<CustomizeRequest>().FirstOrDefaultAsync();
            _output.WriteLine(request.ToString());
            Assert.Equal(CustomizeRequestStatus.Requesting, request.Status);
        }
        [Trait("ReturnTrue", "StaffPricingPending")]
        [Fact]
        public async Task Staff_Pricing_Pending_CustomizeRequest_Should_ReturnPriced()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var pricedResult = await SeedingPricedRequest(account.Id);
            var pricedRequest = await _context.Set<CustomizeRequest>().FirstOrDefaultAsync(p => p.Id == pricedResult.Id);
            Assert.Equal(CustomizeRequestStatus.Priced, pricedResult.Status);
        }
        [Trait("ReturnTrue", "StaffAcceptRequesting")]
        [Fact]
        public async Task Staff_Accept_Requesting_CustomizeRequest_Should_ReturnSuccess()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var acceptedRequest = await SeedingAcceptedRequest(account.Id);
            var request = await _context.Set<CustomizeRequest>().FirstOrDefaultAsync(p => p.Id == acceptedRequest.Id);
            Assert.Equal(CustomizeRequestStatus.Accepted, request.Status);
            var product = await _context.Set<Jewelry>().FirstOrDefaultAsync(p => p.Id == request.JewelryId);
            Assert.NotNull(product);
            Assert.Equal(ProductStatus.Locked,product.Status);
        }
        [Trait("ReturnTrue", "CustomerRejectPriced")]
        [Fact]
        public async Task Customer_Reject_Priced_CustomizeRequest_Should_ReturnSuccess()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var pricedRequest = await SeedingPricedRequest(account.Id);
            Assert.Equal(CustomizeRequestStatus.Priced, pricedRequest.Status);
            var result = await _sender.Send(new CustomerRejectRequestCommand(pricedRequest.Id.Value, account.Id.Value));
            if (result.IsFailed)
                WriteError(result.Errors);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            var request = await _context.Set<CustomizeRequest>().FirstOrDefaultAsync(p => p.Id == pricedRequest.Id);
            Assert.Equal(CustomizeRequestStatus.Customer_Rejected, request.Status);
        }
        [Trait("ReturnTrue", "CustomerRejectAccepted")]
        [Fact]
        public async Task Customer_Reject_Accepting_CustomizeRequest_Should_ReturnSuccess()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var acceptedRequest = await SeedingAcceptedRequest(account.Id);
            Assert.Equal(CustomizeRequestStatus.Accepted, acceptedRequest.Status);
            var result = await _sender.Send(new CustomerRejectRequestCommand(acceptedRequest.Id.Value, account.Id.Value));
            if (result.IsFailed)
                WriteError(result.Errors);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            var request = await _context.Set<CustomizeRequest>().FirstOrDefaultAsync(p => p.Id == acceptedRequest.Id);
            Assert.Equal(CustomizeRequestStatus.Customer_Rejected, request.Status);
            var product = await _context.Set<Jewelry>().FirstOrDefaultAsync(p => p.Id == request.JewelryId);
            Assert.NotNull(product);
            Assert.Equal(ProductStatus.Active, product.Status);
        }
        [Trait("ReturnTrue", "ShopRejectPending")]
        [Fact]
        public async Task Staff_Reject_Pending_CustomizeRequest_Should_ReturnSuccess()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var pendingRequest = await SeedingDefaultModelRequest(account.Id);
            Assert.Equal(CustomizeRequestStatus.Pending, pendingRequest.Status);
            var result = await _sender.Send(new StaffRejectRequestCommand(pendingRequest.Id.Value));
            if (result.IsFailed)
                WriteError(result.Errors);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            var request = await _context.Set<CustomizeRequest>().FirstOrDefaultAsync(p => p.Id == pendingRequest.Id);
            Assert.Equal(CustomizeRequestStatus.Shop_Rejected, request.Status);
        }
        [Trait("ReturnTrue", "ShopRejectRequesting")]
        [Fact]
        public async Task Staff_Reject_Requesting_CustomizeRequest_Should_ReturnSuccess()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var requestingRequest = await SeedingNoDiamondRequest(account.Id);
            Assert.Equal(CustomizeRequestStatus.Requesting, requestingRequest.Status);
            var result = await _sender.Send(new StaffRejectRequestCommand(requestingRequest.Id.Value));
            if (result.IsFailed)
                WriteError(result.Errors);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            var request = await _context.Set<CustomizeRequest>().FirstOrDefaultAsync(p => p.Id == requestingRequest.Id);
            Assert.Equal(CustomizeRequestStatus.Shop_Rejected, request.Status);
        }
    }
}

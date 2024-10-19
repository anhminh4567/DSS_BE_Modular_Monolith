using DiamondShop.Application.Dtos.Requests.Carts;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson;
using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Test.Integration.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace DiamondShop.Test.Integration
{
    public class CartValidatorTest : BaseIntegrationTest
    {
        protected readonly ITestOutputHelper _output;
        public CartValidatorTest(IntegrationWAF factory, ITestOutputHelper output) : base(factory)
        {
            _output = output;
        }
        [Trait("ReturnFalse", "Cart_NoPrice")]
        [Fact]
        public async Task NoPrice_Should_Return_False()
        {
            var diamond = await TestData.SeedDefaultDiamond(_context);
            var items = new CartRequestDto()
            {
                Items = new List<CartItemRequestDto>()
                {
                    new CartItemRequestDto()
                    {
                        DiamondId = diamond.Id.Value
                    }
                }
            };
            var command = new ValidateCartFromListCommand(items);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                foreach (var error in result.Errors)
                    _output.WriteLine(error.Message);
            }
            Assert.True(result.IsSuccess);
            var cartModel = result.Value;
            Assert.False(cartModel.OrderValidation.IsOrderValid);
        }
        [Trait("ReturnFalse", "Cart_WrongMatchingDiamond")]
        [Fact]
        public async Task MismatchingDiamond_Should_Return_False()
        {
            var ring_1 = await TestData.SeedDefaultJewelry(_context, "1", "1");
            var ring_2 = await TestData.SeedDefaultJewelry(_context, "1", "2");
            var diamond = await TestData.SeedDefaultDiamond(_context, ring_1.Id);
            var criteria = await TestData.SeedDefaultDiamondCriteria(_context, diamond.Cut, diamond.Clarity, diamond.Color, diamond.IsLabDiamond);
            await TestData.SeedDefaultDiamondPrice(_context, diamond.DiamondShapeId, criteria.Id);

            var items = new CartRequestDto()
            {
                Items = new List<CartItemRequestDto>()
                {
                    new CartItemRequestDto()
                    {
                        JewelryId = ring_2.Id.Value
                    },
                    new CartItemRequestDto()
                    {
                        JewelryId = ring_2.Id.Value,
                        DiamondId = diamond.Id.Value
                    }
                }
            };
            var command = new ValidateCartFromListCommand(items);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                foreach (var error in result.Errors)
                    _output.WriteLine(error.Message);
            }
            Assert.True(result.IsSuccess);
            var cartModel = result.Value;
            Assert.False(cartModel.OrderValidation.IsOrderValid);
        }
    }
}

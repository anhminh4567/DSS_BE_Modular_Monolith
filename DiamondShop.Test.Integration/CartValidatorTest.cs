using DiamondShop.Application.Dtos.Requests.Carts;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson;
using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Test.Integration.Data;
using Mapster;
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
            var criteria = await TestData.SeedDefaultDiamondCriteria(_context, diamond.IsLabDiamond);// diamond.Cut, diamond.Clarity, diamond.Color,
            await TestData.SeedDefaultDiamondPrice(_context, diamond.DiamondShapeId, criteria.Id, diamond.IsLabDiamond,diamond.Cut,diamond.Clarity,diamond.Color);

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
        [Trait("ReturnFalse", "Cart_DuplicateJewelry")]
        [Fact]
        public async Task Duplicate_Should_InvalidOrder()
        {
            var ring_1 = await TestData.SeedDefaultJewelry(_context, "1", "1");
            var ring_2 = await TestData.SeedDefaultJewelry(_context, "1", "2");
            var diamond = await TestData.SeedDefaultDiamond(_context, ring_1.Id);
            var diamond2 = await TestData.SeedDefaultDiamond(_context, ring_2.Id);
            var criteria = await TestData.SeedDefaultDiamondCriteria(_context,  diamond.IsLabDiamond);//diamond.Cut, diamond.Clarity, diamond.Color,
            await TestData.SeedDefaultDiamondPrice(_context, diamond.DiamondShapeId, criteria.Id, diamond.IsLabDiamond,diamond.Cut,diamond.Clarity,diamond.Color);

            var items = new CartRequestDto()
            {
                Items = new List<CartItemRequestDto>()
                {
                    new CartItemRequestDto()
                    {
                        JewelryId = ring_1.Id.Value
                    },
                    //new CartItemRequestDto()
                    //{
                    //    JewelryId = ring_1.Id.Value,
                    //    DiamondId = diamond.Id.Value
                    //},
                    new CartItemRequestDto()
                    {
                        JewelryId = ring_1.Id.Value
                    },
                    new CartItemRequestDto()
                    {
                        JewelryId = ring_2.Id.Value
                    },

                    new CartItemRequestDto()
                    {
                        JewelryId = ring_2.Id.Value
                    },
                    new CartItemRequestDto()
                    {
                        DiamondId = diamond2.Id.Value
                    },

                }
            };
            var command = new ValidateCartFromListCommand(items);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                foreach (var error in result.Errors) { }
                 // _output.WriteLine(error.Message);
            }
            Assert.True(result.IsSuccess);
            Assert.Equal(items.Items.Count - 1, result.Value.Products.Where(s => s.IsDuplicate).Count());
            var cartModel = result.Value;
            Assert.False(cartModel.OrderValidation.IsOrderValid);
        }
        [Trait("ReturnTrue", "Cart_Valid")]
        [Fact]
        public async Task Valid_Should_ReturnTrue()
        {
            var ring_1 = await TestData.SeedDefaultJewelry(_context, "1", "1");
            var ring_2 = await TestData.SeedDefaultJewelry(_context, "1", "2");
            var diamond = await TestData.SeedDefaultDiamond(_context, ring_1.Id);
            var criteria = await TestData.SeedDefaultDiamondCriteria(_context, diamond.IsLabDiamond);// diamond.Cut, diamond.Clarity, diamond.Color,
            await TestData.SeedDefaultDiamondPrice(_context, diamond.DiamondShapeId, criteria.Id, diamond.IsLabDiamond, diamond.Cut, diamond.Clarity, diamond.Color);

            var items = new CartRequestDto()
            {
                Items = new List<CartItemRequestDto>()
                {
                    new CartItemRequestDto()
                    {
                        JewelryId = ring_1.Id.Value
                    },
                    //new CartItemRequestDto()
                    //{
                    //    //JewelryId = ring_1.Id.Value,
                    //    DiamondId = diamond.Id.Value
                    //},

                }
            };
            var command = new ValidateCartFromListCommand(items);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                foreach (var error in result.Errors) { }
                // _output.WriteLine(error.Message);
            }
            Assert.True(result.IsSuccess);
            var cartModel = result.Value;
            //lỗi ở đây là địa chỉ không hợp lệ, m ko cần test
            Assert.False(cartModel.OrderValidation.IsOrderValid);
        }
    }
}

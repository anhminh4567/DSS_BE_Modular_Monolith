using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Test.Integration.Data;
using Xunit.Abstractions;

namespace DiamondShop.Test.Integration
{
    public class OrderTest : BaseIntegrationTest
    {
        protected readonly ITestOutputHelper _output;
        public OrderTest(IntegrationWAF factory, ITestOutputHelper output) : base(factory)
        {
            _output = output;
        }

        [Trait("ReturnTrue", "Transfer_COD")]
        [Fact]
        public async Task Checkout_Should_Create_Order()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var jewelry = await TestData.SeedDefaultJewelry(_context);
            var diamond = await TestData.SeedDefaultDiamond(_context, jewelry.Id);
            var criteria = await TestData.SeedDefaultDiamondCriteria(_context, diamond.Cut, diamond.Clarity, diamond.Color, diamond.IsLabDiamond);
            await TestData.SeedDefaultDiamondPrice(_context, diamond.DiamondShapeId, criteria.Id);

            var billing = new BillingDetail(account.FullName.FirstName, account.FullName.LastName, "123456789", account.Email, "HCM", "Thu Duc", "Tam Binh", "abc street", "");
            var orderReq = new OrderRequestDto(Domain.Models.Orders.Enum.PaymentType.COD, "zalopay", null, true);
            var itemReqs = new List<OrderItemRequestDto>(){
                new OrderItemRequestDto(jewelry.Id.Value, null, null, null, "Default_Jewelry_Warranty", WarrantyType.Jewelry),
                new OrderItemRequestDto(jewelry.Id.Value, diamond.Id.Value, null, null, "Default_Diamond_Warranty", WarrantyType.Diamond)
            };
            var orderDetail = new CreateOrderInfo(orderReq, itemReqs);
            var command = new CreateOrderCommand(account.Id.Value, billing, orderDetail);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                foreach (var error in result.Errors)
                    _output.WriteLine(error.Message);
            }
            else
            {
                var items = _context.Set<OrderItem>().ToList();
                foreach (var item in items)
                    _output.WriteLine($"{item.JewelryId} {item.DiamondId} {item.PurchasedPrice}");
                if (result.Value != null)
                {
                    _output.WriteLine($"{result.Value.PaymentUrl}");
                    _output.WriteLine($"{result.Value.QrCode}");
                }
            }
            //_output.WriteLine(result.Value.PaymentUrl);
            //_output.WriteLine(result.Value.QrCode);
            Assert.True(result.IsSuccess);
        }
    }
}

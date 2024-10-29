using DiamondShop.Api.Controllers.Orders.Cancel;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Usecases.Orders.Commands.Checkout;
using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Application.Usecases.Orders.Commands.Proceed;
using DiamondShop.Application.Usecases.Orders.Commands.Reject;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Test.Integration.Data;
using FluentResults;
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
        void WriteError(List<IError> errors)
        {

            foreach (var error in errors)
                _output.WriteLine(error.Message);
        }
        async Task<Jewelry> SeedingOrderItem()
        {
            var jewelry = await TestData.SeedDefaultJewelry(_context);
            var diamond = await TestData.SeedDefaultDiamond(_context, jewelry.Id);
            var criteria = await TestData.SeedDefaultDiamondCriteria(_context, diamond.Cut, diamond.Clarity, diamond.Color, diamond.IsLabDiamond);
            await TestData.SeedDefaultDiamondPrice(_context, diamond.DiamondShapeId, criteria.Id, diamond.IsLabDiamond);
            return jewelry;
        }
        async Task<Order> SeedingPendingOrder()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var jewelry = await SeedingOrderItem();
            var itemReqs = new List<OrderItemRequestDto>(){
                new OrderItemRequestDto(jewelry.Id.Value, null, null, null, "Default_Jewelry_Warranty", WarrantyType.Jewelry),
            };
            var address = String.Join(" ", ["HCM", "Thu Duc", "Tam Binh", "abc street"]);
            var orderDetail = new CreateOrderInfo(Domain.Models.Orders.Enum.PaymentType.Payall, "zalopay", "", address, itemReqs);
            var createCommand = new CreateOrderCommand(account.Id.Value, orderDetail);
            var createResult = await _sender.Send(createCommand);
            if (createResult.IsFailed)
            {
                WriteError(createResult.Errors);
            }
            Assert.True(createResult.IsSuccess);
            Assert.NotNull(createResult.Value);
            return createResult.Value;
        }
        async Task<Order> SeedingProcessingOrder(string accountId)
        {
            var order = await SeedingPendingOrder();
            var processingCommand = new ProceedOrderCommand(order.Id.Value, accountId);
            var processingResult = await _sender.Send(processingCommand);
            if (processingResult.IsFailed)
            {
                WriteError(processingResult.Errors);
            }
            Assert.True(processingResult.IsSuccess);
            Assert.NotNull(processingResult.Value);
            order = processingResult.Value;
            Assert.Equal(OrderStatus.Processing, order.Status);
            if (order.PaymentType == PaymentType.Payall)
                Assert.Equal(PaymentStatus.PaidAll, order.PaymentStatus);
            else
                Assert.Equal(PaymentStatus.Deposited, order.PaymentStatus);
            return order;
        }
        [Trait("ReturnTrue", "Transfer_COD")]
        [Fact]
        public async Task Checkout_Should_Create_Order()
        {
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var jewelry = await SeedingOrderItem();
            var billing = new BillingDetail(account.FullName.FirstName, account.FullName.LastName, "123456789", account.Email, "HCM", "Thu Duc", "Tam Binh", "abc street", "");
            var orderReq = new OrderRequestDto(PaymentType.COD, "zalopay", null, true);
            var itemReqs = new List<OrderItemRequestDto>(){
                new OrderItemRequestDto(jewelry.Id.Value, null, null, null, "Default_Jewelry_Warranty", WarrantyType.Jewelry),
            };
            var orderDetail = new CheckoutOrderInfo(orderReq, itemReqs);
            var command = new CheckoutOrderCommand(account.Id.Value, billing, orderDetail);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                WriteError(result.Errors);
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
        [Trait("ReturnTrue", "CancelPendingOrder")]
        [Fact]
        public async Task Customer_Cancel_Order_When_Pending_Should_Refund_All()
        {
            var order = await SeedingPendingOrder();
            Assert.Equal(PaymentType.Payall, order.PaymentType);
            Assert.Equal(OrderStatus.Pending, order.Status);
            if (order != null)
            {
                _output.WriteLine($"{order.Status}");
                var cancelCommand = new CancelOrderCommand(order.Id.Value, order.AccountId.Value, "just canceling");
                var cancelResult = await _sender.Send(cancelCommand);
                if (cancelResult.IsFailed)
                {
                    WriteError(cancelResult.Errors);
                }
                else
                {
                    if (cancelResult.Value != null)
                    {
                        _output.WriteLine($"{cancelResult.Value.Status}");
                    }
                }
                Assert.True(cancelResult.IsSuccess);
                //check payment
                var transacs = _context.Set<Transaction>().ToList();
                foreach (var transac in transacs)
                {
                    _output.WriteLine($"{transac.TransactionType} {transac.IsManual} {transac.TotalAmount} {transac.FineAmount} {transac.TransactionAmount}");
                }
                //Assert that refund only 100%
                var payAmount = transacs.Where(p => p.TransactionType == TransactionType.Pay).Sum(p => p.TotalAmount);
                var refundAmount = transacs.Where(p => p.TransactionType == TransactionType.Refund || p.TransactionType == TransactionType.Partial_Refund).Sum(p => p.TotalAmount);
                Assert.Equal(MoneyVndRoundUpRules.RoundAmountFromDecimal(payAmount), refundAmount);
                Assert.Equal(OrderStatus.Cancelled, order.Status);
                Assert.Equal(PaymentStatus.Refunding, order.PaymentStatus);
            }
        }
        [Trait("ReturnTrue", "RejectPendingOrder")]
        [Fact]
        public async Task Shop_Reject_Order_When_Pending_Should_Refund_All()
        {
            var order = await SeedingPendingOrder();
            Assert.Equal(PaymentType.Payall, order.PaymentType);
            Assert.Equal(OrderStatus.Pending, order.Status);
            if (order != null)
            {
                _output.WriteLine($"{order.Status}");
                var rejectCommand = new RejectOrderCommand(order.Id.Value, order.AccountId.Value, "just rejecting");
                var rejectResult = await _sender.Send(rejectCommand);
                if (rejectResult.IsFailed)
                {
                    WriteError(rejectResult.Errors);
                }
                else
                {
                    if (rejectResult.Value != null)
                    {
                        _output.WriteLine($"{rejectResult.Value.Status}");
                    }
                }
                Assert.True(rejectResult.IsSuccess);
                //check payment
                var transacs = _context.Set<Transaction>().ToList();
                foreach (var transac in transacs)
                {
                    _output.WriteLine($"{transac.TransactionType} {transac.IsManual} {transac.TotalAmount} {transac.FineAmount} {transac.TransactionAmount}");
                }
                //Assert that refund only 100%
                var payAmount = transacs.Where(p => p.TransactionType == TransactionType.Pay).Sum(p => p.TotalAmount);
                var refundAmount = transacs.Where(p => p.TransactionType == TransactionType.Refund || p.TransactionType == TransactionType.Partial_Refund).Sum(p => p.TotalAmount);
                Assert.Equal(MoneyVndRoundUpRules.RoundAmountFromDecimal(payAmount), refundAmount);
                Assert.Equal(OrderStatus.Rejected, order.Status);
                Assert.Equal(PaymentStatus.Refunding, order.PaymentStatus);
            }
        }
        [Trait("ReturnTrue", "CancelNonPendingOrder")]
        [Fact]
        public async Task Customer_Cancel_Order_After_Pending_Should_Refund_Partial()
        {

            var staff = await TestData.SeedDefaultStaff(_context, _authentication);
            var order = await SeedingProcessingOrder(staff.Id.Value);
            Assert.Equal(PaymentType.Payall, order.PaymentType);
            Assert.Equal(OrderStatus.Processing, order.Status);
            if (order != null)
            {
                _output.WriteLine($"{order.Status}");
                var cancelCommand = new CancelOrderCommand(order.Id.Value, order.AccountId.Value, "just canceling");
                var cancelResult = await _sender.Send(cancelCommand);
                if (cancelResult.IsFailed)
                {
                    WriteError(cancelResult.Errors);
                }
                else
                {
                    if (cancelResult.Value != null)
                    {
                        _output.WriteLine($"{cancelResult.Value.Status}");
                    }
                }
                Assert.True(cancelResult.IsSuccess);
                //check payment
                var transacs = _context.Set<Transaction>().ToList();
                foreach (var transac in transacs)
                {
                    _output.WriteLine($"{transac.TransactionType} {transac.IsManual} {transac.TotalAmount} {transac.FineAmount} {transac.TransactionAmount}");
                }
                //Assert that refund only BR amount
                var payAmount = transacs.Where(p => p.TransactionType == TransactionType.Pay).Sum(p => p.TotalAmount);
                var refundAmount = transacs.Where(p => p.TransactionType == TransactionType.Refund || p.TransactionType == TransactionType.Partial_Refund).Sum(p => p.TotalAmount);
                if (order.PaymentType == PaymentType.Payall)
                    Assert.Equal(MoneyVndRoundUpRules.RoundAmountFromDecimal(payAmount * (1m - 0.01m * OrderPaymentRules.PayAllFine)), refundAmount);
                Assert.Equal(OrderStatus.Cancelled, order.Status);
                Assert.Equal(PaymentStatus.Refunding, order.PaymentStatus);
            }
        }
        [Trait("ReturnTrue", "RejectNonPendingOrder")]
        [Fact]
        public async Task Shop_Reject_Order_After_Pending_Should_Refund_All()
        {
            var staff = await TestData.SeedDefaultStaff(_context, _authentication);
            var order = await SeedingProcessingOrder(staff.Id.Value);
            Assert.Equal(PaymentType.Payall, order.PaymentType);
            Assert.Equal(OrderStatus.Processing, order.Status);
            if (order != null)
            {
                _output.WriteLine($"{order.Status}");
                var rejectCommand = new RejectOrderCommand(order.Id.Value, order.AccountId.Value, "just rejecting");
                var rejectResult = await _sender.Send(rejectCommand);
                if (rejectResult.IsFailed)
                {
                    WriteError(rejectResult.Errors);
                }
                else
                {
                    if (rejectResult.Value != null)
                    {
                        _output.WriteLine($"{rejectResult.Value.Status}");
                    }
                }
                Assert.True(rejectResult.IsSuccess);
                //check payment
                var transacs = _context.Set<Transaction>().ToList();
                foreach (var transac in transacs)
                {
                    _output.WriteLine($"{transac.TransactionType} {transac.IsManual} {transac.TotalAmount} {transac.FineAmount} {transac.TransactionAmount}");
                }
                //Assert that refund only BR amount
                var payAmount = transacs.Where(p => p.TransactionType == TransactionType.Pay).Sum(p => p.TotalAmount);
                var refundAmount = transacs.Where(p => p.TransactionType == TransactionType.Refund || p.TransactionType == TransactionType.Partial_Refund).Sum(p => p.TotalAmount);
                Assert.Equal(MoneyVndRoundUpRules.RoundAmountFromDecimal(payAmount), refundAmount);
                Assert.Equal(OrderStatus.Rejected, order.Status);
                Assert.Equal(PaymentStatus.Refunding, order.PaymentStatus);
            }
        }
    }
}

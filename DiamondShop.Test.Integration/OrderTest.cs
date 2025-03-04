﻿using DiamondShop.Api.Controllers.Orders.AssignDeliverer;
using DiamondShop.Api.Controllers.Orders.Cancel;
using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Application.Services.Interfaces.Transfers;
using DiamondShop.Application.Usecases.Diamonds.Commands.Delete;
using DiamondShop.Application.Usecases.Jewelries.Commands.Delete;
using DiamondShop.Application.Usecases.Orders.Commands.Checkout;
using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Application.Usecases.Orders.Commands.DeliverEnd;
using DiamondShop.Application.Usecases.Orders.Commands.DeliverFail;
using DiamondShop.Application.Usecases.Orders.Commands.Proceed;
using DiamondShop.Application.Usecases.Orders.Commands.Refund;
using DiamondShop.Application.Usecases.Orders.Commands.Reject;
using DiamondShop.Application.Usecases.Orders.Commands.Transfer.Deliverer;
using DiamondShop.Application.Usecases.Orders.Commands.Transfer.Manager;
using DiamondShop.Application.Usecases.Orders.Commands.Transfer.Staff;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Locations;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.Events;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.Models.Transactions.ErrorMessages;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Test.Integration.Data;
using FluentResults;
using HtmlAgilityPack.CssSelectors.NetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
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
        async Task fakeStaffRefundTransfer(Order order, AccountId staffId)
        {
            var transactions = _context.Set<Transaction>().Where(p => p.OrderId == order.Id && p.IsManual == true && p.Status == TransactionStatus.Valid).ToList();
            Assert.NotEmpty(transactions);
            var refundAmount = transactions.Sum(p => p.TransactionAmount);
            if (order.Status == OrderStatus.Cancelled && order.PaymentType == PaymentType.Payall)
                refundAmount = MoneyVndRoundUpRules.RoundAmountFromDecimal(refundAmount * (1m - 0.01m * OrderPaymentRules.Default.PayAllFine));

            var refundPayment = Transaction.CreateManualRefund(order.Id,"ACB","123", staffId, "ABCCCDDD", $"Hoàn tiền đến khách hàng {order.Account?.FullName.FirstName} {order.Account?.FullName.LastName} cho đơn hàng ${order.OrderCode}", refundAmount);
            await _context.Set<Transaction>().AddAsync(refundPayment);
            order.PaymentStatus = PaymentStatus.Refunded;
            _context.Set<Order>().Update(order);
            await _context.SaveChangesAsync();
        }
        async Task<Transaction> fakeCustomerTransfer(Order order)
        {
            var existed = _context.Set<Transaction>().Any(p => p.OrderId == order.Id && p.IsManual == true && p.TransactionType == TransactionType.Pay);
            Assert.False(existed);
            var payAmount = order.PaymentType == PaymentType.Payall ? order.TotalPrice : order.DepositFee;
            var manualPayment = Transaction.CreateManualPayment(order.Id, "ACB", "123", $"Chuyển khoản từ tài khoản {order.Account?.FullName.FirstName} {order.Account?.FullName.LastName} cho đơn hàng {order.OrderCode}", payAmount, TransactionType.Pay);
            manualPayment.Evidence = Media.Create("evidence", "http/test-evidence", "");
            await _context.Set<Transaction>().AddAsync(manualPayment);
            await _context.SaveChangesAsync();
            var transaction = _context.Set<Transaction>().FirstOrDefault(p => p.OrderId == order.Id);
            return transaction;
        }
        async Task<Transaction> fakeDelivererConfirmTransfer(Order order, AccountId delivererId)
        {
            Assert.Equal(PaymentType.COD, order.PaymentType);
            var transactions = _context.Set<Transaction>().Where(p => p.OrderId == order.Id && p.Status == TransactionStatus.Valid).ToList();
            Assert.NotEmpty(transactions);
            //remaing amount
            var payAmount = order.TotalPrice - order.DepositFee;
            var manualPayment = Transaction.CreateManualPayment(order.Id, "ACB", "123", $"Chuyển tiền còn lại từ tài khoản {order.Account?.FullName.FirstName} {order.Account?.FullName.LastName} cho đơn hàng {order.OrderCode}", payAmount, TransactionType.Pay);
            //add evidence to blob
            await _context.Set<Transaction>().AddAsync(manualPayment);
            await _context.SaveChangesAsync();
            return manualPayment;
        }
        async Task<Order> fakeDelivererCompleteDelivery(Order order, AccountId delivererId)
        {
            var items = order.Items;
            foreach (var item in items)
            {
                item.Status = OrderItemStatus.Done;
            }
            order.Status = OrderStatus.Success;
            _context.Set<Order>().Update(order);
            await _context.SaveChangesAsync();
            return order;
        }
        async Task<Jewelry> SeedingOrderJewelry()
        {
            var jewelry = await TestData.SeedDefaultJewelry(_context);
            var diamond = await TestData.SeedDefaultDiamond(_context, jewelry.Id);
            var criteria = await TestData.SeedDefaultDiamondCriteria(_context, diamond.IsLabDiamond);//diamond.Cut, diamond.Clarity, diamond.Color,
            await TestData.SeedDefaultDiamondPrice(_context, diamond.DiamondShapeId, criteria.Id, diamond.IsLabDiamond, diamond.Cut, diamond.Clarity, diamond.Color, 1_000M);
            return jewelry;
        }
        async Task<Order> SeedingPendingOrder(string accountId, PaymentType paymentType = PaymentType.Payall)
        {
            var city = new AppCities()
            {
                Slug = "HOCHIMINH",
                Name = "Hồ Chí Minh",
                Type = 1
            };
            await _context.AppCities.AddAsync(city);
            var fee = DeliveryFee.CreateLocationType("", 30_00, "Hồ Chí Minh", city.Id);
            await _context.Set<DeliveryFee>().AddAsync(fee);
            var jewelry = await SeedingOrderJewelry();
            var diamond2 = await TestData.SeedDefaultDiamond(_context);
            var itemReqs = new List<OrderItemRequestDto>(){
                new OrderItemRequestDto(jewelry.Id.Value, null, null, null, "Default_Jewelry_Warranty", WarrantyType.Jewelry),
                new OrderItemRequestDto(null, diamond2.Id.Value, null, null, "Default_Diamond_Warranty", WarrantyType.Diamond)
            };
            var billingDetail = new BillingDetail("abc", "abc", "123123132", "abc@gmail.com", "Hồ Chí Minh", "Thu Duc", "Ward", "abc street", "no");
            var address = String.Join(" ", ["Hồ Chí Minh", "Thu Duc", "Tam Binh", "abc street"]);
            var orderDetail = new CreateOrderInfo(paymentType, PaymentMethod.BANK_TRANSFER.Id.Value, "zalopay", null, null, billingDetail, itemReqs);
            var createCommand = new CreateOrderCommand(accountId, orderDetail);
            var createResult = await _sender.Send(createCommand);
            if (createResult.IsFailed)
            {
                WriteError(createResult.Errors);
            }
            Assert.True(createResult.IsSuccess);
            Assert.NotNull(createResult.Value);
            return createResult.Value;
        }
        async Task<Order> SeedingProcessingOrder(string accountId, string managerId, PaymentType paymentType = PaymentType.Payall)
        {
            var order = await SeedingPendingOrder(accountId, paymentType);
            var transaction = await fakeCustomerTransfer(order);
            Assert.NotNull(transaction);
            Assert.Equal(TransactionStatus.Verifying, transaction.Status);
            var processingResult = await _sender.Send(new StaffConfirmPendingTransferCommand(managerId, new(transaction.Id.Value, transaction.TotalAmount, "ABCDEFG")));
            if (processingResult.IsFailed)
            {
                WriteError(processingResult.Errors);
            }
            Assert.True(processingResult.IsSuccess);
            Assert.NotNull(processingResult.Value);
            order = processingResult.Value;
            Assert.Equal(OrderStatus.Processing, order.Status);
            return order;
        }
        async Task<Order> SeedingPreparedOrder(string accountId, string managerId = null, PaymentType paymentType = PaymentType.Payall)
        {
            if (managerId == null)
                managerId = (await TestData.SeedDefaultManager(_context, _authentication)).Id.Value;
            var order = await SeedingProcessingOrder(accountId, managerId, paymentType);
            var processingResult = await _sender.Send(new ProceedOrderCommand(order.Id.Value, accountId));
            if (processingResult.IsFailed)
            {
                WriteError(processingResult.Errors);
            }
            Assert.True(processingResult.IsSuccess);
            Assert.NotNull(processingResult.Value);
            Assert.Equal(OrderStatus.Prepared, order.Status);
            return order;
        }
        async Task<Order> SeedingDeliveringOrder(string accountId, string delivererId, string managerId, PaymentType paymentType = PaymentType.Payall)
        {
            var order = await SeedingPreparedOrder(accountId, managerId, paymentType);
            var assignCommand = new AssignDelivererOrderCommand(order.Id.Value, delivererId);
            var assignResult = await _sender.Send(assignCommand);
            if (assignResult.IsFailed)
            {
                WriteError(assignResult.Errors);
            }
            Assert.True(assignResult.IsSuccess);
            Assert.NotNull(assignResult.Value);
            Assert.Equal(delivererId, assignResult.Value.DelivererId?.Value);
            var processingCommand = new ProceedOrderCommand(order.Id.Value, accountId);
            var processingResult = await _sender.Send(processingCommand);
            if (processingResult.IsFailed)
            {
                WriteError(processingResult.Errors);
            }
            Assert.True(processingResult.IsSuccess);
            Assert.NotNull(processingResult.Value);
            order = processingResult.Value;
            Assert.Equal(OrderStatus.Delivering, order.Status);
            return order;
        }
        [Trait("ReturnTrue", "Transfer_COD")]
        [Fact()]
        public async Task Checkout_Should_Create_Order()
        {
            var city = new AppCities()
            {
                Slug = "HOCHIMINH",
                Name = "Hồ Chí Minh",
                Type = 1
            };
            await _context.AppCities.AddAsync(city);
            var fee = DeliveryFee.CreateLocationType("", 30_00, "Hồ Chí Minh", city.Id);
            await _context.Set<DeliveryFee>().AddAsync(fee);
            var account = await TestData.SeedDefaultCustomer(_context, _authentication);
            var jewelry = await SeedingOrderJewelry();
            var billingDetail = new BillingDetail("abc", "abc", "0123987456", "abc@gmail.com", "Hồ Chí Minh", "Thu Duc", "Ward", "Tan Binh", "no");
            var orderReq = new OrderRequestDto(PaymentType.COD, PaymentMethod.BANK_TRANSFER.Id.Value, "zalopay", null, true);
            var itemReqs = new List<OrderItemRequestDto>(){
                new OrderItemRequestDto(jewelry.Id.Value, null, null, null, "Default_Jewelry_Warranty", WarrantyType.Jewelry),
            };
            var orderDetail = new CheckoutOrderInfo(orderReq, itemReqs);
            var command = new CheckoutOrderCommand(account.Id.Value, billingDetail, orderDetail);
            var result = await _sender.Send(command);

            if (result.IsFailed)
            {
                WriteError(result.Errors);
            }
            else
            {
                _output.WriteLine(account.PhoneNumber.ToString());
                var items = _context.Set<OrderItem>().ToList();
                foreach (var item in items)
                    _output.WriteLine($"{item.JewelryId} {item.DiamondId} {item.PurchasedPrice}");
                if (result.Value != null)
                {
                    _output.WriteLine($"{result.Value.PaymentUrl}");
                    _output.WriteLine($"{result.Value.QrCode}");
                }
            }
            Assert.True(result.IsSuccess);
        }
        [Trait("ReturnTrue", "Verify_Transfer")]
        [Fact()]
        public async Task Manager_Verify_Transfer_Should_Make_Order_Processing()
        {
            var city = new AppCities()
            {
                Slug = "HOCHIMINH",
                Name = "Hồ Chí Minh",
                Type = 1
            };
            await _context.AppCities.AddAsync(city);
            var fee = DeliveryFee.CreateLocationType("", 30_00, "Hồ Chí Minh", city.Id);
            await _context.Set<DeliveryFee>().AddAsync(fee);
            var customer = await TestData.SeedDefaultCustomer(_context, _authentication);
            var manager = await TestData.SeedDefaultManager(_context, _authentication);
            var jewelry = await SeedingOrderJewelry();
            var billingDetail = new BillingDetail("abc", "abc", "123123132", "abc@gmail.com", "Hồ Chí Minh", "Thu Duc", "Ward", "Tan Binh", "no");
            var orderReq = new OrderRequestDto(PaymentType.COD, PaymentMethod.BANK_TRANSFER.Id.Value, "zalopay", null, true);
            var itemReqs = new List<OrderItemRequestDto>(){
                new OrderItemRequestDto(jewelry.Id.Value, null, null, null, "Default_Jewelry_Warranty", WarrantyType.Jewelry),
            };
            var orderDetail = new CheckoutOrderInfo(orderReq, itemReqs);
            var command = new CheckoutOrderCommand(customer.Id.Value, billingDetail, orderDetail);
            var result = await _sender.Send(command);
            if (result.IsFailed)
            {
                WriteError(result.Errors);
            }
            Assert.True(result.IsSuccess);
            //Customer send proof of transfer
            //Fake customerTransferOrderCommand
            var order = _context.Set<Order>().FirstOrDefault(p => p.AccountId == customer.Id);
            Assert.NotNull(order);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Equal(PaymentMethod.BANK_TRANSFER.Id, order.PaymentMethodId);
            Assert.False(DateTime.UtcNow > order.ExpiredDate);
            var existed = _context.Set<Transaction>().Any(p => p.OrderId == order.Id && p.IsManual == true && p.TransactionType == TransactionType.Pay);
            Assert.False(existed);
            var payAmount = order.PaymentType == PaymentType.Payall ? order.TotalPrice : order.DepositFee;
            var manualPayment = Transaction.CreateManualPayment(order.Id, "ACB", "123", $"Chuyển khoản từ tài khoản {order.Account?.FullName.FirstName} {order.Account?.FullName.LastName} cho đơn hàng {order.OrderCode}", payAmount, TransactionType.Pay);
            manualPayment.Evidence = Media.Create("evidence", "http/test-evidence", "");
            await _context.Set<Transaction>().AddAsync(manualPayment);

            _context.Set<Order>().Update(order);
            await _context.SaveChangesAsync();
            //
            var transaction = _context.Set<Transaction>().FirstOrDefault(p => p.OrderId == order.Id);
            Assert.NotNull(transaction);
            Assert.Equal(TransactionStatus.Verifying, transaction.Status);
            var managerCompleteResult = await _sender.Send(new StaffConfirmPendingTransferCommand(manager.Id.Value, new(transaction.Id.Value, transaction.TotalAmount, "ABCDEFG")));
            if (managerCompleteResult.IsFailed)
            {
                WriteError(managerCompleteResult.Errors);
            }
            Assert.True(managerCompleteResult.IsSuccess);
            //transaction = _context.Set<Transaction>().FirstOrDefault(p => p.OrderId == order.Id);
            Assert.NotNull(transaction);
            Assert.Equal(transaction.VerifierId, manager.Id);
            _output.WriteLine(transaction.AppTransactionCode);
            Assert.Equal(TransactionStatus.Valid, transaction.Status);
            Assert.Equal(OrderStatus.Processing, order.Status);
        }
        [Trait("ReturnTrue", "PayAll_Order")]
        [Fact()]
        public async Task Complete_PayAll_Order_Should_Return_SUCCESS()
        {
            var staff = await TestData.SeedDefaultStaff(_context, _authentication);
            var manager = await TestData.SeedDefaultManager(_context, _authentication);
            var deliverer = await TestData.SeedDefaultDeliverer(_context, _authentication);
            var order = await SeedingDeliveringOrder(staff.Id.Value, deliverer.Id.Value, manager.Id.Value, PaymentType.Payall);
            if (order != null)
            {
                _output.WriteLine($"{order.Status}");
                //var completeCommand = new ProceedOrderCommand(order.Id.Value, deliverer.Id.Value);
                await fakeDelivererCompleteDelivery(order, deliverer.Id);
                //check payment
                var transacs = _context.Set<Transaction>().ToList();
                _output.WriteLine($"Order total price = {order.TotalPrice}");
                var sumAmount = 0m;
                foreach (var transac in transacs)
                {
                    sumAmount += transac.TransactionType == TransactionType.Pay ? transac.TransactionAmount : 0m;
                    _output.WriteLine($"{transac.TransactionType} {transac.IsManual} {transac.TransactionAmount} {transac.VerifierId.Value}");
                }
                Assert.Equal(sumAmount, order.TotalPrice);
            }
        }
        [Trait("ReturnTrue", "COD_Order")]
        [Fact()]
        public async Task Complete_COD_Order_Should_Return_SUCCESS()
        {
            var staff = await TestData.SeedDefaultStaff(_context, _authentication);
            var manager = await TestData.SeedDefaultManager(_context, _authentication);
            var deliverer = await TestData.SeedDefaultDeliverer(_context, _authentication);
            var order = await SeedingDeliveringOrder(staff.Id.Value, deliverer.Id.Value, manager.Id.Value, PaymentType.COD);
            if (order != null)
            {
                _output.WriteLine($"{order.Status}");
                //Deliverer send proof of remaining transfer
                var remainingTrans = await fakeDelivererConfirmTransfer(order, deliverer.Id);
                var remainingTransferResult = await _sender.Send(new StaffConfirmDeliveringTransferCommand(manager.Id.Value, new(remainingTrans.Id.Value, remainingTrans.TransactionAmount, "ABCDERGF")));
                if (remainingTransferResult.IsFailed)
                {
                    WriteError(remainingTransferResult.Errors);
                }
                Assert.True(remainingTransferResult.IsSuccess);
                await fakeDelivererCompleteDelivery(order, deliverer.Id);
                //check payment
                var transacs = _context.Set<Transaction>().ToList();
                _output.WriteLine($"Order total price = {order.TotalPrice}");
                var sumAmount = 0m;
                foreach (var transac in transacs)
                {
                    sumAmount += transac.TransactionType == TransactionType.Pay ? transac.TransactionAmount : 0m;
                    _output.WriteLine($"{transac.TransactionType} {transac.IsManual} {transac.TransactionAmount} {transac.VerifierId}");
                }
                Assert.Equal(sumAmount, order.TotalPrice);
            }
        }
        [Trait("ReturnTrue", "CancelNonPendingOrder")]
        [Fact()]
        public async Task Customer_Cancel_Order_After_Pending_Should_Refund_CANCELLED()
        {

            var manager = await TestData.SeedDefaultManager(_context, _authentication);
            var customer = await TestData.SeedDefaultCustomer(_context, _authentication);
            var order = await SeedingProcessingOrder(customer.Id.Value, manager.Id.Value);
            Assert.Equal(PaymentType.Payall, order.PaymentType);
            Assert.Equal(OrderStatus.Processing, order.Status);
            _output.WriteLine("Before cancel");
            var diamonds = _context.Set<Diamond>().ToList();
            var jewelries = _context.Set<Jewelry>().ToList();
            foreach (Jewelry jewelry in jewelries)
            {
                _output.WriteLine($"{jewelry.Id.Value} - {jewelry.Status} - {jewelry.TotalPrice} - {jewelry.SoldPrice} - {jewelry.D_Price} - {jewelry.ND_Price}");
            }
            foreach (Diamond diamond in diamonds)
                _output.WriteLine($"{diamond.Id.Value} - {diamond.Status} - {diamond.JewelryId?.Value} - {diamond.TruePrice} - {diamond.SoldPrice}");

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
                Assert.Equal(PaymentStatus.Refunding, order.PaymentStatus);
                await fakeStaffRefundTransfer(order, manager.Id);
                var transacs = _context.Set<Transaction>().ToList();
                foreach (var transac in transacs)
                {
                    _output.WriteLine($"{transac.TransactionType} {transac.IsManual} {transac.TotalAmount} {transac.FineAmount} {transac.TransactionAmount}");
                }
                //Assert that refund only BR amount
                var payAmount = transacs.Where(p => p.TransactionType == TransactionType.Pay).Sum(p => p.TotalAmount);
                var refundAmount = transacs.Where(p => p.TransactionType == TransactionType.Refund).Sum(p => p.TransactionAmount);
                if (order.PaymentType == PaymentType.Payall)
                    Assert.Equal(MoneyVndRoundUpRules.RoundAmountFromDecimal(payAmount * (1m - 0.01m * OrderPaymentRules.Default.PayAllFine)), refundAmount);
                Assert.Equal(OrderStatus.Cancelled, order.Status);
                Assert.Equal(PaymentStatus.Refunded, order.PaymentStatus);
                jewelries = _context.Set<Jewelry>().ToList();
                _output.WriteLine("After cancel");
                foreach (Jewelry jewelry in jewelries)
                    _output.WriteLine($"{jewelry.Id} - {jewelry.Status}");
                diamonds = _context.Set<Diamond>().ToList();
                foreach (Diamond diamond in diamonds)
                    _output.WriteLine($"{diamond.Id} - {diamond.Status}");
                var items = await _context.Set<OrderItem>().ToListAsync();
                foreach (var item in items)
                {
                    _output.WriteLine($"{item.JewelryId?.Value}-{item.DiamondId?.Value} {item.ProductId} {item.Name} {item.IsProductDelete}");
                }
            }
        }
        [Trait("ReturnTrue", "RejectNonPendingOrder")]
        [Fact()]
        public async Task Shop_Reject_Order_After_Pending_Should_Refund_REJECTED()
        {
            var manager = await TestData.SeedDefaultManager(_context, _authentication);
            var customer = await TestData.SeedDefaultCustomer(_context, _authentication);
            var order = await SeedingProcessingOrder(customer.Id.Value, manager.Id.Value);
            Assert.Equal(PaymentType.Payall, order.PaymentType);
            Assert.Equal(OrderStatus.Processing, order.Status);
            if (order != null)
            {
                _output.WriteLine($"{order.Status}");
                var rejectCommand = new RejectOrderCommand(order.Id.Value, "just rejecting");
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
                Assert.Equal(PaymentStatus.Refunding, order.PaymentStatus);
                await fakeStaffRefundTransfer(order, manager.Id);
                var transacs = _context.Set<Transaction>().ToList();
                foreach (var transac in transacs)
                {
                    _output.WriteLine($"{transac.TransactionType} {transac.IsManual} {transac.TotalAmount} {transac.FineAmount} {transac.TransactionAmount}");
                }
                //Assert that refund only BR amount
                var payAmount = transacs.Where(p => p.TransactionType == TransactionType.Pay).Sum(p => p.TotalAmount);
                var refundAmount = transacs.Where(p => p.TransactionType == TransactionType.Refund).Sum(p => p.TransactionAmount);
                Assert.Equal(MoneyVndRoundUpRules.RoundAmountFromDecimal(payAmount), refundAmount);
                Assert.Equal(OrderStatus.Rejected, order.Status);
                Assert.Equal(PaymentStatus.Refunded, order.PaymentStatus);
            }

        }
        [Trait("ReturnTrue", "CompleteRefund")]
        [Fact()]
        public async Task Order_Complete_Refund_Should_Return_REFUNDED()
        {
            var manager = await TestData.SeedDefaultManager(_context, _authentication);
            var customer = await TestData.SeedDefaultCustomer(_context, _authentication);
            var order = await SeedingProcessingOrder(customer.Id.Value, manager.Id.Value);
            Assert.Equal(PaymentType.Payall, order.PaymentType);
            Assert.Equal(OrderStatus.Processing, order.Status);
            if (order != null)
            {
                _output.WriteLine($"{order.Status}");
                var rejectCommand = new RejectOrderCommand(order.Id.Value, "just rejecting");
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
                Assert.Equal(PaymentStatus.Refunding, order.PaymentStatus);
                await fakeStaffRefundTransfer(order, manager.Id);
                var transacs = _context.Set<Transaction>().ToList();
                foreach (var transac in transacs)
                {
                    _output.WriteLine($"{transac.TransactionType} {transac.IsManual} {transac.TotalAmount} {transac.FineAmount} {transac.TransactionAmount}");
                }
                //Assert that refund only BR amount
                var payAmount = transacs.Where(p => p.TransactionType == TransactionType.Pay).Sum(p => p.TotalAmount);
                var refundAmount = transacs.Where(p => p.TransactionType == TransactionType.Refund).Sum(p => p.TransactionAmount);
                if (order.PaymentType == PaymentType.Payall)
                    Assert.Equal(MoneyVndRoundUpRules.RoundAmountFromDecimal(payAmount), refundAmount);
                Assert.Equal(OrderStatus.Rejected, order.Status);
            }
        }
        [Trait("ReturnTrue", "RejectPayAllTransferPendingOrder")]
        [Fact()]
        public async Task Shop_Reject_PayAll_Transfer_While_Pending_Should_NoRefund_CANCELLED()
        {
            var manager = await TestData.SeedDefaultManager(_context, _authentication);
            var customer = await TestData.SeedDefaultCustomer(_context, _authentication);
            var order = await SeedingPendingOrder(customer.Id.Value);
            var transaction = await fakeCustomerTransfer(order);
            Assert.NotNull(transaction);
            Assert.Equal(TransactionStatus.Verifying, transaction.Status);
            var rejectTransferResult = await _sender.Send(new StaffRejectTransferCommand(manager.Id.Value, new(transaction.Id.Value)));
            if (rejectTransferResult.IsFailed)
            {
                WriteError(rejectTransferResult.Errors);
            }
            Assert.True(rejectTransferResult.IsSuccess);
            Assert.NotNull(rejectTransferResult.Value);
            order = rejectTransferResult.Value;
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.Equal(PaymentStatus.No_Refund, order.PaymentStatus);
            var items = await _context.Set<OrderItem>().ToListAsync();
            foreach (var item in items)
            {
                _output.WriteLine($"{item.JewelryId?.Value}-{item.DiamondId?.Value} {item.ProductId} {item.Name} {item.IsProductDelete}");
            }
        }
        [Trait("ReturnTrue", "RejectCODTransferPendingOrder")]
        [Fact()]
        public async Task Shop_Reject_COD_Transfer_While_Pending_Should_NoRefund_CANCELLED()
        {
            var manager = await TestData.SeedDefaultManager(_context, _authentication);
            var customer = await TestData.SeedDefaultCustomer(_context, _authentication);
            var order = await SeedingPendingOrder(customer.Id.Value, PaymentType.COD);
            var transaction = await fakeCustomerTransfer(order);
            Assert.NotNull(transaction);
            Assert.Equal(TransactionStatus.Verifying, transaction.Status);
            var rejectTransferResult = await _sender.Send(new StaffRejectTransferCommand(manager.Id.Value, new(transaction.Id.Value)));
            if (rejectTransferResult.IsFailed)
            {
                WriteError(rejectTransferResult.Errors);
            }
            Assert.True(rejectTransferResult.IsSuccess);
            Assert.NotNull(rejectTransferResult.Value);
            order = rejectTransferResult.Value;
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.Equal(PaymentStatus.No_Refund, order.PaymentStatus);
        }
        [Trait("ReturnTrue", "RejectTransferDeliveringOrder")]
        [Fact()]
        public async Task Shop_Reject_COD_Transfer_While_Delivering_Should_NoRefund_CANCELLED()
        {
            var staff = await TestData.SeedDefaultStaff(_context, _authentication);
            var manager = await TestData.SeedDefaultManager(_context, _authentication);
            var deliverer = await TestData.SeedDefaultDeliverer(_context, _authentication);
            var order = await SeedingDeliveringOrder(staff.Id.Value, deliverer.Id.Value, manager.Id.Value, PaymentType.COD);
            if (order != null)
            {
                _output.WriteLine($"{order.Status}");
                //Deliverer send proof of remaining transfer
                var remainingTrans = await fakeDelivererConfirmTransfer(order, deliverer.Id);
                var rejectTransferResult = await _sender.Send(new StaffRejectTransferCommand(manager.Id.Value, new(remainingTrans.Id.Value)));
                if (rejectTransferResult.IsFailed)
                {
                    WriteError(rejectTransferResult.Errors);
                }
                Assert.True(rejectTransferResult.IsSuccess);
                Assert.NotNull(rejectTransferResult.Value);
                order = rejectTransferResult.Value;
                Assert.Equal(OrderStatus.Delivering, order.Status);
                Assert.Equal(PaymentStatus.Deposited, order.PaymentStatus);
                Assert.False(order.HasDelivererReturned);
                var deliveryFailResult = await _sender.Send(new OrderDeliverFailCommand(deliverer.Id.Value, new(order.Id.Value)));
                if (deliveryFailResult.IsFailed)
                {
                    WriteError(deliveryFailResult.Errors);
                }
                Assert.Equal(OrderStatus.Delivery_Failed, order.Status);
                var deliveryEndResult = await _sender.Send(new OrderDeliverEndCommand(order.Id.Value));
                if (deliveryEndResult.IsFailed)
                {
                    WriteError(deliveryEndResult.Errors);
                }
                Assert.True(order.HasDelivererReturned);
                var rejectResult = await _sender.Send(new RejectOrderCommand(order.Id.Value, "huy", true));
                if (rejectResult.IsFailed)
                {
                    WriteError(rejectResult.Errors);
                }
                Assert.Equal(OrderStatus.Cancelled, order.Status);
                Assert.Equal(PaymentStatus.No_Refund, order.PaymentStatus);
                var items = await _context.Set<OrderItem>().ToListAsync();
                foreach(var item in items)
                {
                    _output.WriteLine($"{item.JewelryId?.Value}-{item.DiamondId?.Value} {item.ProductId} {item.Name} {item.IsProductDelete}");
                }
            }
        }
        [Trait("ReturnTrue", "Test delete jewelry-item")]
        [Fact()]
        public async Task DeleteJewelry()
        {
            var manager = await TestData.SeedDefaultManager(_context, _authentication);
            var customer = await TestData.SeedDefaultCustomer(_context, _authentication);
            var order = await SeedingPendingOrder(customer.Id.Value, PaymentType.COD);
            var transaction = await fakeCustomerTransfer(order);
            Assert.NotNull(transaction);
            Assert.Equal(TransactionStatus.Verifying, transaction.Status);
            var rejectTransferResult = await _sender.Send(new StaffRejectTransferCommand(manager.Id.Value, new(transaction.Id.Value)));
            if (rejectTransferResult.IsFailed)
            {
                WriteError(rejectTransferResult.Errors);
            }
            Assert.True(rejectTransferResult.IsSuccess);
            Assert.NotNull(rejectTransferResult.Value);
            order = rejectTransferResult.Value;
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.Equal(PaymentStatus.No_Refund, order.PaymentStatus);
            order = await _context.Set<Order>().Include(p => p.Items).FirstOrDefaultAsync();
            foreach (var item in order.Items)
            {
                _output.WriteLine($"{item.JewelryId?.Value}-{item.DiamondId?.Value} {item.ProductId} {item.Name} {item.IsProductDelete}");
                if (item.JewelryId != null)
                {
                    var deleteFlag = await _sender.Send(new DeleteJewelryCommand(item.JewelryId.Value));
                    WriteError(deleteFlag.Errors);

                    Assert.True(deleteFlag.IsSuccess);
                }
                if (item.DiamondId != null)
                {
                    var deleteFlag = await _sender.Send(new DeleteDiamondCommand(item.DiamondId.Value));
                    WriteError(deleteFlag.Errors);
                    Assert.True(deleteFlag.IsSuccess);
                }
            }
            order = await _context.Set<Order>().Include(p => p.Items).FirstOrDefaultAsync();
            foreach (var item in order.Items)
            {
                _output.WriteLine($"{item.JewelryId?.Value}-{item.DiamondId?.Value} {item.ProductId} {item.Name} {item.IsProductDelete}");
            }
        }
    }
}

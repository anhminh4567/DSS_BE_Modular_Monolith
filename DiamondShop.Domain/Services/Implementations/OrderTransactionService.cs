using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace DiamondShop.Domain.Services.Implementations
{
    public class OrderTransactionService : IOrderTransactionService
    {
        private readonly ILogger<OrderTransactionService> _logger;
        private readonly ITransactionRepository _transactionRepository;

        public OrderTransactionService(ILogger<OrderTransactionService> logger, ITransactionRepository transactionRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        public decimal GetCODValueForOrder(Order order)
        {
            if (order.PaymentType != Models.Orders.Enum.PaymentType.COD)
            {
                throw new Exception("this is not of type COD ");
            }
            var codPercent = OrderPaymentRules.Default.CODPercent;
            var neededToPayAmountRaw = order.TotalPrice * (Decimal.Divide(codPercent, 100));
            var roundedValue = MoneyVndRoundUpRules.RoundAmountFromDecimal(neededToPayAmountRaw); //Math.Round(Decimal.Divide(neededToPayAmountRaw, 1000), 1) * 1000;//the function is tested in linq pad
            return roundedValue;
        }

        public decimal GetCorrectAmountFromOrder(Order order)
        {
            decimal? getCorrectAmount = null;
            Func<Order, decimal> getRemainingForCOD = (Order inputOrder) =>
            {
                if (inputOrder.PaymentStatus == PaymentStatus.Deposited && inputOrder.PaymentType == PaymentType.COD)
                    return GetRemaingValueForOrder(inputOrder);
                else if (inputOrder.PaymentStatus == PaymentStatus.Pending && inputOrder.PaymentType == PaymentType.COD)
                {
                    if (inputOrder.IsCustomOrder)
                        return GetDepositValueForOrder(inputOrder);
                    return GetCODValueForOrder(inputOrder);
                }
                else
                    throw new Exception("the order is not deposited or is not COD  to get the remaining");
            };
            if (order.IsCustomOrder)
            {
                if (order.PaymentType == PaymentType.Payall)
                    getCorrectAmount = GetFullPaymentValueForOrder(order);

                else if (order.PaymentType == PaymentType.COD)
                    getCorrectAmount = getRemainingForCOD(order);

                else
                    throw new Exception("unspecified payment type");
            }
            else
            {
                if (order.PaymentType == PaymentType.Payall)
                    getCorrectAmount = GetFullPaymentValueForOrder(order);

                else if (order.PaymentType == PaymentType.COD)
                    getCorrectAmount = getRemainingForCOD(order);
                else
                    throw new Exception("unspecified payment type");
            }
            if (getCorrectAmount is null)
                throw new Exception("the correct amount is null");
            return MoneyVndRoundUpRules.RoundAmountFromDecimal(getCorrectAmount.Value);
        }

        public decimal GetDepositValueForOrder(Order order)
        {
            if (order.PaymentType != Models.Orders.Enum.PaymentType.COD)
            {
                throw new Exception("this is not of type COD ");
            }
            var depositPercent = OrderPaymentRules.Default.DepositPercent;
            var neededToPayAmountRaw = order.TotalPrice * (Decimal.Divide(depositPercent, 100));
            var roundedValue = MoneyVndRoundUpRules.RoundAmountFromDecimal(neededToPayAmountRaw); //Math.Round(Decimal.Divide(neededToPayAmountRaw, 1000), 1) * 1000;//the function is tested in linq pad
            return roundedValue;
        }

        public decimal GetFullPaymentValueForOrder(Order order)
        {
            return order.TotalPrice;
        }

        public decimal GetRefundAmountFromOrder(Order order, decimal fineAmount, OrderPaymentRules rule)
        {
            if (order.PaymentStatus == Models.Orders.Enum.PaymentStatus.Refunded)
            {
                return -1;
            }
            var sumTransactions = order.Transactions
                .Where(t => t.TransactionType == TransactionType.Pay && t.Status == TransactionStatus.Valid)
                .Sum(x => x.TotalAmount);
            //var refundTrans = order.Transactions
            //    .Where(t => t.TransactionType == TransactionType.Refund && t.Status == TransactionStatus.Valid)
            //    .Sum(x => x.TotalAmount);
            //var leftAmount = sumTransactions - refundTrans;
            decimal refundAmount = 0;
            if (order.Status == OrderStatus.Cancelled)
            {
                if (order.PaymentType == PaymentType.Payall)
                {
                    var fine = MoneyVndRoundUpRules.RoundAmountFromDecimal(sumTransactions * ( 0.01m * rule.PayAllFine));
                    refundAmount = sumTransactions - fine;
                }
                else
                {
                    var fine = order.DepositFee;
                    refundAmount = sumTransactions - fine;
                }
            }
            else if(order.Status == OrderStatus.Rejected)
            {
                refundAmount = sumTransactions;
            }
            else
            {
                return -1;
            }
            return refundAmount;
        }

        public void AddRefundShopReject(Order order, AccountId VerifierId, string TransactionCode, OrderStatus previousStatus)
        {
            decimal refundAmount = 0;
            decimal finedAmount = 0;
            List<Transaction> transactions = new();
            if (order.Status == OrderStatus.Pending)
            {
                order.PaymentStatus = PaymentStatus.Refunded;
                return;
            }
            if (previousStatus != OrderStatus.Pending)
            {
                //TODO: Calculate in case of second order 
                transactions = order.Transactions
                     .Where(p => p.TransactionType == TransactionType.Pay
                     //|| p.TransactionType == TransactionType.Pay_Remain
                     //|| p.TransactionType == TransactionType.Deposit
                     )
                     .Where(p => p.Status == TransactionStatus.Valid).ToList();
                var transaction = transactions.FirstOrDefault();
                if (transaction == null)
                    throw new Exception("No transaction found");

                bool isDeposit = order.PaymentStatus == PaymentStatus.Deposited;
                if (isDeposit)
                {
                    var totalPaid = transactions.Sum(p => p.TotalAmount);
                    refundAmount = totalPaid;//transactions.Sum(p => p.TotalAmount);
                    finedAmount = 0;
                }
                else
                {
                    var totalPaid = transactions.Sum(p => p.TotalAmount);
                    refundAmount = totalPaid;
                    refundAmount = MoneyVndRoundUpRules.RoundAmountFromDecimal(refundAmount);
                    order.TotalFine = 0;//+= totalPaid - refundAmount;
                    finedAmount = 0;//refundAmount - totalPaid;
                }
                var transac = Transaction.CreateManualRefund(order.Id, VerifierId, TransactionCode, $"Manual refund for order#{order.OrderCode}", refundAmount);
                order.AddRefund(transac);
            }
            else
            {
                order.PaymentStatus = PaymentStatus.Refunded;
                return;
            }

        }
        public void AddRefundUserCancel(Order order, AccountId VerifierId, string TransactionCode, OrderStatus previousStatus)
        {
            decimal refundAmount = 0;
            decimal finedAmount = 0;
            List<Transaction> transactions = new();
            if (order.Status == OrderStatus.Pending)
            {
                order.PaymentStatus = PaymentStatus.Refunded;
                return;
            }
            if (previousStatus != OrderStatus.Pending)
            {
                //TODO: Calculate in case of second order 
                transactions = order.Transactions
                     .Where(p => p.TransactionType == TransactionType.Pay).ToList();
                var transaction = transactions.FirstOrDefault();
                if (transaction == null)
                    throw new Exception("No transaction found");
                bool isDeposit = transactions.Sum(x => x.TotalAmount) < order.TotalPrice;
                if (isDeposit)
                {
                    // day la so am do Total = amount - fine ==> de total > 0 thi fine < 0
                    var totalPaid = transactions.Sum(p => p.TotalAmount);
                    finedAmount = 0;// -transactions.Sum(p => p.TotalAmount);
                    order.TotalFine += totalPaid;
                    refundAmount = 0; // lay luon coc neu bi phat
                }
                else
                {
                    var totalPaid = transactions.Sum(p => p.TotalAmount);
                    refundAmount = totalPaid * (1m - 0.01m * OrderPaymentRules.Default.PayAllFine);
                    refundAmount = MoneyVndRoundUpRules.RoundAmountFromDecimal(refundAmount);
                    order.TotalFine += totalPaid - refundAmount;
                    finedAmount = 0;
                    //finedAmount = -(refundAmount - totalPaid);
                }
                var transac = Transaction.CreateManualRefund(order.Id, VerifierId, TransactionCode, $"hoàn tiền cho đơn hàng mã #{order.OrderCode}", refundAmount);
                order.AddRefund(transac);
                if (transac.TransactionAmount == 0)
                {
                    transac.Status = TransactionStatus.Valid;
                    transac.Description = "Không hoàn tiền do dat coc";
                    order.PaymentStatus = PaymentStatus.Refunded;
                }
            }
            else
            {
                order.PaymentStatus = PaymentStatus.Refunded;
                return;
            }
        }

        public void AddCODPayment(Order order)
        {
            var transaction = order.Transactions.FirstOrDefault(p => p.TransactionType == TransactionType.Pay);
            if (transaction == null)
                throw new Exception("No transaction found");
            var remainAmount = order.TotalPrice - transaction.TotalAmount;
            if (transaction.IsManual)
            {
                var transac = Transaction.CreateManualPayment(order.Id, $"trả phần còn lại cho đơn hàng mã #{order.OrderCode}", remainAmount, TransactionType.Pay);
                order.AddTransaction(transac);
                order.PaymentStatus = PaymentStatus.Paid;
            }
        }
        public decimal GetRefundUserCancelAfterDelivery(Order order)
        {
            throw new NotImplementedException();
        }

        public decimal GetRefundUserCancelBeforeProcessing(Order order)
        {
            throw new NotImplementedException();
        }

        public decimal GetRefundUserCancelDuringProcessingAndPrepared(Order order)
        {
            throw new NotImplementedException();
        }

        public decimal GetRemaingValueForOrder(Order order)
        {
            var transactions = order.Transactions;
            var paidAmount = transactions
                .Where(x => x.TransactionType == TransactionType.Pay && x.Status == TransactionStatus.Valid)
                .Sum(x => x.TransactionAmount);
            var depositAmount = order.DepositFee;
            return order.TotalPrice - paidAmount;
        }

        public Result<(decimal allowAmount, decimal remainingAmount)> GetTransactionValueForOrder(Order order, decimal wantedAmount, TransactionRule transactionRule)
        {
            var orderTotal = order.TotalPrice;
            var remaining = orderTotal - wantedAmount;
            if (wantedAmount > transactionRule.MaximumPerTransaction)
                return Result.Fail($"the wanted to pay amount is: {wantedAmount} , which is not in our rules is maximum = {transactionRule.MaximumPerTransaction}");
            if (remaining < transactionRule.MinimumPerTransaction)
                return Result.Fail($"the remaining money is: {remaining} , which is not in our rules is minimum = {transactionRule.MinimumPerTransaction}");
            return Result.Ok((wantedAmount, remaining));
        }

    }
}

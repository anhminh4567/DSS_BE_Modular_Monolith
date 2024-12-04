using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public record ExternalBankQrcodeDto(string? qrCode, string qrImage);
    public record ExternalBankTransactionDetailDto(string shopAccountNum, string? userAccountNum, long recordedAmount, string description, string code);
    public interface IExternalBankServices
    {
        ExternalBankQrcodeDto GenerateQrCodeFromOrder(Order order, decimal amount);
        ExternalBankTransactionDetailDto GetTransactionDetail(Order order, Transaction transaction);

    }
}

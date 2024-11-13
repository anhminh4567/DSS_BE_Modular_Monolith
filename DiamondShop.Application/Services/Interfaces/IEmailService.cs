using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Orders;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task<Result> Send(string toEmail, string title, string description,string bodyContentHtml, CancellationToken cancellationToken = default);
        Task<Result> SendConfirmAccountEmail(Account user, string token, CancellationToken cancellationToken = default);
        Task<Result> SendInvoiceEmail(Order order, Account account, CancellationToken cancellationToken = default);
    }
    public record EmailVerificationMailModel(Account userAccount, DateTime createdTime, DateTime expiredTime, string token,string callbackUrl);
}

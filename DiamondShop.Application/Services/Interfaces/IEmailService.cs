using DiamondShop.Domain.Models.AccountAggregate;
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
        Task<Result> Send(string toEmail, string title, string description,string bodyContentHtml);
    }
    public record EmailVerificationToken(Account userAccount, DateTime createdTime, DateTime expiredTime);
}

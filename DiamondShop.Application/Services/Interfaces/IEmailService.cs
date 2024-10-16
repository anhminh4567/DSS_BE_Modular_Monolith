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
}

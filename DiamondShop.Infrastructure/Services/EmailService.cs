using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Infrastructure.Options;
using FluentEmail.Core;
using FluentResults;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services
{
    internal class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmailServices;
        private readonly IOptions<MailOptions> _options;

        public EmailService(IFluentEmail fluentEmailServices, IOptions<MailOptions> options)
        {
            _fluentEmailServices = fluentEmailServices;
            _options = options;
        }


        public async Task<Result> Send(string toEmail, string title, string description, string bodyContentHtml)
        {
            var emailSendingConfig = _fluentEmailServices
             .To(toEmail)
             .Subject(title);
            return Result.Ok();
        }
    }
}

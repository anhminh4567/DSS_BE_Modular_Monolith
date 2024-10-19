using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Securities;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentResults;
using Microsoft.AspNetCore.Identity;
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
        private readonly IOptions<MailOptions> _mailOptions;
        private readonly IOptions<UrlOptions> _urlOptions;
        private readonly CustomUserManager _userManager;
        private const string ConfirmEmailFileName = "ConfirmEmailTemplate.cshtml";
        private const string IconFileName = "ShopIcon.png";

        public EmailService(IFluentEmail fluentEmailServices, IOptions<MailOptions> mailOptions, IOptions<UrlOptions> urlOptions, CustomUserManager userManager)
        {
            _fluentEmailServices = fluentEmailServices;
            _mailOptions = mailOptions;
            _urlOptions = urlOptions;
            _userManager = userManager;
        }

        public Task<Result> Send(string toEmail, string title, string description, string bodyContentHtml, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async  Task<Result> SendConfirmAccountEmail(Account user, CancellationToken cancellationToken = default)
        {
            var callbackUrl = _urlOptions.Value.HttpsUrl + "/" + _mailOptions.Value.ConfirmCallbackEndpoint;
            var getconfirmationEmailTemplate = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","EmailTemplate", ConfirmEmailFileName);
            if (getconfirmationEmailTemplate == null)
                return Result.Fail(new NotFoundError());
            //var secretCode = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            var metadata = new EmailMetaData()
            {
                ToEmail = user.Email,
                Subject = "confirmation email",
            };
            var model = new EmailVerificationMailModel(user, DateTime.UtcNow,DateTime.UtcNow.AddDays(1),"testtoekn", callbackUrl);
            return await SendEmailWithTemplate(metadata, getconfirmationEmailTemplate, model, cancellationToken);
            throw new NotImplementedException();
        }
        private async Task<Result> SendEmailWithTemplate<T>(EmailMetaData emailMetaData, string templatePath, T templateModel, CancellationToken cancellation = default)
        {
            var emailSendingConfig = _fluentEmailServices
                .To(emailMetaData.ToEmail)
                .Subject(emailMetaData.Subject);
            emailSendingConfig.UsingTemplateFromFile(templatePath, templateModel);
            emailSendingConfig.Attach(new Attachment()
            {
                ContentId = "logo",
                ContentType = "image/png",
                Data = new MemoryStream(File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","Images", IconFileName))),
                IsInline = true,
                Filename = IconFileName,
            });
            var sendResult = await emailSendingConfig.SendAsync(cancellation);
            if (sendResult.Successful is false)
                return Result.Fail("cant send email");
            return Result.Ok();
        }
    }
    public class EmailMetaData
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public IEnumerable<string>? Bccs { get; set; }   // do not required if it is to single person
        public IEnumerable<string>? Ccs { get; set; }
        public string? BodyString { get; set; }// do not require if the body is html
        public string? AttachmentPath { get; set; }
        public string? LogoImageBase64 { get; set; }
        public IList<EmailAttachments>? Attachments { get; set; }// require only when the send with attachment is used
    }
    public class EmailAttachments
    {
        public Stream FileStream { get; set; }
        public string ContentType { get; set; }
        public string? FileName { get; set; }
    }
}

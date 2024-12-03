using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Securities;
using DiamondShop.Infrastructure.Services.Pdfs;
using DiamondShop.Infrastructure.Services.Pdfs.Models;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentResults;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services
{
    internal class EmailService : IEmailService
    {
        private readonly ILogger<EmailService   > _logger;
        private readonly IFluentEmail _fluentEmailServices;
        private readonly IOptions<MailOptions> _mailOptions;
        private readonly IOptions<UrlOptions> _urlOptions;
        private readonly IOptions<PublicBlobOptions> _publicBlobOptions;
        private readonly IOptions<ExternalUrlsOptions> _externalUrlsOptions;
        private readonly CustomUserManager _userManager;
        private const string ConfirmEmailFileName = "ConfirmEmailTemplate.cshtml";
        private const string InvoiceEmailFileName = "OrderInvoiceTemplate.cshtml";
        private const string OrderPreparedEmailFileName = "OrderPreparedNotificationEmailTemplate.cshtml";
        private const string IconFileName = "ShopIcon.png";
        private readonly IPdfService _pdfService;
        private readonly IOptions<FrontendOptions> _frontendOptions;

        public EmailService(ILogger<EmailService> logger, IFluentEmail fluentEmailServices, IOptions<MailOptions> mailOptions, IOptions<UrlOptions> urlOptions, IOptions<PublicBlobOptions> publicBlobOptions, IOptions<ExternalUrlsOptions> externalUrlsOptions, CustomUserManager userManager, IPdfService pdfService, IOptions<FrontendOptions> frontendOptions)
        {
            _logger = logger;
            _fluentEmailServices = fluentEmailServices;
            _mailOptions = mailOptions;
            _urlOptions = urlOptions;
            _publicBlobOptions = publicBlobOptions;
            _externalUrlsOptions = externalUrlsOptions;
            _userManager = userManager;
            _pdfService = pdfService;
            _frontendOptions = frontendOptions;
        }

        public Task<Result> Send(string toEmail, string title, string description, string bodyContentHtml, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async  Task<Result> SendConfirmAccountEmail(Account user, string token, CancellationToken cancellationToken = default)
        {
            var callbackUrl = _urlOptions.Value.HttpsUrl + "/" + _mailOptions.Value.ConfirmCallbackEndpoint;
            var getconfirmationEmailTemplate = Path.Combine(Directory.GetCurrentDirectory(), "RazorTemplate", "EmailTemplate", ConfirmEmailFileName);
            if (getconfirmationEmailTemplate == null)
                return Result.Fail(new NotFoundError());
            //var secretCode = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            var metadata = new EmailMetaData()
            {
                ToEmail = user.Email,
                Subject = "confirmation email",
            };
            var model = new EmailVerificationMailModel(user, DateTime.UtcNow,DateTime.UtcNow.AddDays(1), token, callbackUrl);
            return await SendEmailWithTemplate(metadata, getconfirmationEmailTemplate, model, cancellationToken);
            //throw new NotImplementedException();
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

        public async Task<Result> SendInvoiceEmail(Order order, Account account, CancellationToken cancellationToken = default)
        {
            try
            {
                var invoiceEmail = Path.Combine(Directory.GetCurrentDirectory(), "RazorTemplate", "EmailTemplate", ConfirmEmailFileName);
                var mailString = _pdfService.GetTemplateHtmlStringFromOrder(order, account);
                var metadata = new EmailMetaData()
                {
                    ToEmail = account.Email,
                    Subject = "invoice for order email",
                };
                var emailSendingConfig = _fluentEmailServices
                  .To(metadata.ToEmail)
                  .Subject(metadata.Subject)
                  .Body(mailString, isHtml: true);
                //emailSendingConfig.Attach(new Attachment()
                //{
                //    ContentId = "invoice",
                //    ContentType = "application/pdf",
                //    Data = GeneratePdfService.GeneratePdfDoc(mailString),
                //    IsInline = false,
                //    Filename = $"invoice_{order.OrderCode}.pdf",
                //});
                var sendResult = await emailSendingConfig.SendAsync();
                if (sendResult.Successful is false)
                    return Result.Fail("cant send email");
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result.Fail(ex.Message);
            }
        }

        public async Task<Result> SendOrderPreparedEmail(Order order, Account account, DateTime completeeDate, CancellationToken cancellationToken = default)
        {
            var getOrderPreparedEmail = Path.Combine(Directory.GetCurrentDirectory(), "RazorTemplate", "EmailTemplate", OrderPreparedEmailFileName);
            if (getOrderPreparedEmail == null)
                return Result.Fail(new NotFoundError());
            //var secretCode = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            var metadata = new EmailMetaData()
            {
                ToEmail = account.Email,
                Subject = "order prepared",
            };
            var orderDetailurl = _frontendOptions.Value.OrderDetailUrl+ "?id"+ order.Id;
            var totalTransAmount = order.Transactions.Where(x => x.TransactionType == TransactionType.Pay || x.TransactionType == TransactionType.Pay_Remain)
                .Sum(x => x.TransactionAmount);
            var model = new EmailOrderPreparedModel(order, account, completeeDate, orderDetailurl, totalTransAmount);
            return await SendEmailWithTemplate(metadata, getOrderPreparedEmail, model, cancellationToken);
            throw new NotImplementedException();
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

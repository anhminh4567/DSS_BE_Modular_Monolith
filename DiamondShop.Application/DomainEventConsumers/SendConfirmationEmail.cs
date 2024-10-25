using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.Events;
using DiamondShop.Domain.Repositories;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.DomainEventConsumers
{
    internal class SendConfirmationEmail : INotificationHandler<CustomerCreatedMessage>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAuthenticationService _authenticationService;

        public SendConfirmationEmail(IAccountRepository accountRepository, IAuthenticationService authenticationService)
        {
            _accountRepository = accountRepository;
            _authenticationService = authenticationService;
        }

        public async Task Handle(CustomerCreatedMessage notification, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetById(notification.AccountId);
            if(account is null)
            {
                return;
            }
            var result =await _authenticationService.SendConfirmEmail(account.Id.Value);
            if (result.IsFailed)
                throw new Exception($"fail to send email to user, reasons: {result.Errors.First().Message}");
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.Events;
using DiamondShop.Domain.Repositories;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IAccountRepository _accountRepository;

        public SendConfirmationEmail(IUnitOfWork unitOfWork, IEmailService emailService, IAccountRepository accountRepository)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _accountRepository = accountRepository;
        }

        public async Task Handle(CustomerCreatedMessage notification, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetById(notification.AccountId);
            if(account is null)
            {
                return;
            }

            throw new NotImplementedException();
        }
    }
}

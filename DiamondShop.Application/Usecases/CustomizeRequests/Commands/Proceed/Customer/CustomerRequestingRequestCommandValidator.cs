using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Customer
{
    public class CustomerRequestingRequestCommandValidator : AbstractValidator<CustomerRequestingRequestCommand>
    {
        public CustomerRequestingRequestCommandValidator()
        {
            RuleFor(p => p.CustomizeRequestId).NotEmpty();
            RuleFor(p => p.AccountId).NotEmpty();
        }
    }
}

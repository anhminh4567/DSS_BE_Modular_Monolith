using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Reject.Customer
{
    public class CustomerRejectRequestCommandValidator : AbstractValidator<CustomerRejectRequestCommand>
    {
        public CustomerRejectRequestCommandValidator() { 
            RuleFor(p => p.CustomizeRequestId).NotEmpty();
            RuleFor(p => p.AccountId).NotEmpty();
        }
    }
}

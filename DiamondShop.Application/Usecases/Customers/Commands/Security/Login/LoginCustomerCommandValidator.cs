using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Customers.Commands.Security.Login
{
    internal class LoginCustomerCommandValidator : AbstractValidator<LoginCustomerCommand>
    {
        public LoginCustomerCommandValidator()
        {
            RuleFor(q => q.email).NotEmpty().EmailAddress();
            RuleFor(q => q.password).NotEmpty();
        }
    }
}

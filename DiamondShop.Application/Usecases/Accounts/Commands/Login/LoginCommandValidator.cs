using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Accounts.Commands.Login
{
    internal class LoginCommandValidator : AbstractValidator<LoginCommand>  
    {
        public LoginCommandValidator()
        {
            RuleFor(q => q.email).NotEmpty().EmailAddress();
            RuleFor(q => q.password).NotEmpty();
            RuleFor(q => q.isStaffLogin).NotNull();
            RuleFor(q => q.isExternalLogin).NotNull();

        }
    }
}

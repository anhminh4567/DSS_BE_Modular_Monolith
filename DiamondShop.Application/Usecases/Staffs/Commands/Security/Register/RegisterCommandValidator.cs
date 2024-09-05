using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Staffs.Commands.Security.Register
{
    internal class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(c => c.email).NotEmpty().EmailAddress();
            RuleFor(c => c.password).NotEmpty();

            RuleFor(c => c.fullName.FirstName).NotEmpty();
            RuleFor(c => c.fullName.LastName).NotEmpty();
            RuleFor(c => c.fullName).NotNull();

            RuleFor(c => c.isManager).NotNull();
        }
    }
}

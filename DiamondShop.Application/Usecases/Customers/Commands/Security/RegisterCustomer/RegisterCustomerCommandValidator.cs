using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Customers.Commands.Security.RegisterCustomer
{
    public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
    {
        public RegisterCustomerCommandValidator()
        {
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
            RuleFor(c => c.Password).NotEmpty();
            RuleFor(c => c.FullName.FirstName).NotEmpty();
            RuleFor(c => c.FullName.LastName).NotEmpty();
            RuleFor(c => c.FullName).NotNull();
        }
    }
}

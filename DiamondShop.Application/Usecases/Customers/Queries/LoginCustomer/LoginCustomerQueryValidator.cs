using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Customers.Queries.LoginCustomer
{
    internal class LoginCustomerQueryValidator : AbstractValidator<LoginCustomerQuery>
    {
        public LoginCustomerQueryValidator()
        {
            RuleFor(q => q.email).NotEmpty().EmailAddress();
            RuleFor(q => q.password).NotEmpty();
        }
    }
}

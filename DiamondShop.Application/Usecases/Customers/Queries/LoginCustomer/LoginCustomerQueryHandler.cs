using DiamondShop.Application.Commons.Responses;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Customers.Queries.LoginCustomer
{
    public record LoginCustomerQuery(string email, string password) : IRequest<Result<AuthenticationResultDto>>;
    internal class LoginCustomerQueryHandler : IRequestHandler<LoginCustomerQuery, Result<AuthenticationResultDto>>
    {
        public Task<Result<AuthenticationResultDto>> Handle(LoginCustomerQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

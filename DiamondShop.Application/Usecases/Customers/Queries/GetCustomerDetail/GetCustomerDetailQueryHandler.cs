using BeatvisionRemake.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.CustomerAggregate;
using DiamondShop.Domain.Models.CustomerAggregate.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Customers.Queries.GetCustomerDetail
{
    public record GetCustomerDetailQuery() : IRequest<Result<Customer>>;
    internal class GetCustomerDetailQueryHandler : IRequestHandler<GetCustomerDetailQuery, Result<Customer>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public GetCustomerDetailQueryHandler(ICustomerRepository customerRepository, IHttpContextAccessor contextAccessor)
        {
            _customerRepository = customerRepository;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<Customer>> Handle(GetCustomerDetailQuery request, CancellationToken cancellationToken)
        {
            HttpContext? httpContext = _contextAccessor.HttpContext;
            if (httpContext == null)
                throw new Exception("http context not found, unknown reasons");
            var claims = httpContext.User.Claims.ToList() ;
            Claim? getIdentityClaim = claims.FirstOrDefault(c => c.Type == IJwtTokenProvider.IDENTITY_CLAIM_NAME);
            if (getIdentityClaim == null)
                return Result.Fail(new NotFoundError("not found identity of this user")) ;
            Customer? getCustomerDetail = await _customerRepository.GetByIdentityId(getIdentityClaim.Value,cancellationToken);
            if (getCustomerDetail == null)
                return Result.Fail(new NotFoundError("noit found user with this identity"));
            return getCustomerDetail;

        }
    }
}

using DiamondShop.Domain.Models.CustomerAggregate;
using DiamondShop.Domain.Models.CustomerAggregate.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Customers.Queries.GetCustomerDetail
{
    public record GetCustomerDetailQuery(CustomerId CustomerId) : IRequest<Result<Customer>>;
    internal class GetCustomerDetailQueryHandler : IRequestHandler<GetCustomerDetailQuery, Result<Customer>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerDetailQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Result<Customer>> Handle(GetCustomerDetailQuery request, CancellationToken cancellationToken)
        {
            return await _customerRepository.GetById(cancellationToken,request.CustomerId);
        }
    }
}

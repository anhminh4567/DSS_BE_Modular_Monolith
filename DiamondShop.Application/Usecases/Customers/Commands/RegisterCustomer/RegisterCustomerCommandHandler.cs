using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.CustomerAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Roles;
using FluentResults;
using MediatR;
using System;
using System.Windows.Input;

namespace DiamondShop.Application.Usecases.Customers.Commands.RegisterCustomer
{
    public record RegisterCustomerCommand : IRequest<Result<Customer>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public FullName? FullName { get; set; }
    }


    internal class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, Result<Customer>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAuthorizationService _authorizationService;

        public RegisterCustomerCommandHandler(IAuthenticationService authenticationService, IUnitOfWork unitOfWork, ICustomerRepository customerRepository, IAuthorizationService authorizationService)
        {
            _authenticationService = authenticationService;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _authorizationService = authorizationService;
        }

        public async Task<Result<Customer>> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = Customer.Create(request.FullName, request.Email);

            await _unitOfWork.BeginTransactionAsync();
            var registerResult = await _authenticationService.Register(request.Email,request.Password,request.FullName,cancellationToken);
            if(registerResult.IsSuccess is false) 
            {
                return Result.Fail(registerResult.Errors);
            }
            string identityId = registerResult.Value;
            IUserIdentity identity = ( await _authenticationService.GetUserIdentity(identityId) ).Value;
            await _authorizationService.AddToRole(identity, DiamondShopCustomerRole.Customer);
            customer.SetIdentity(identity);
            _customerRepository.Create(customer).Wait();
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            var getNewUser = await _customerRepository.GetById(cancellationToken,customer.Id);
            
            return Result.Ok(getNewUser);
        }
    }
}

using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Windows.Input;

namespace DiamondShop.Application.Usecases.Accounts.Commands.RegisterCustomer
{
    public record RegisterCustomerCommand(string? Email, string? Password, FullName? FullName, bool isExternalRegister = false) : IRequest<Result<AuthenticationResultDto>>;



    internal class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, Result<AuthenticationResultDto>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRoleRepository _accountRoleRepository;
        private readonly IAccountRepository _accountRepository;

        public RegisterCustomerCommandHandler(IAuthenticationService authenticationService, IUnitOfWork unitOfWork, IAccountRoleRepository accountRoleRepository, IAccountRepository accountRepository)
        {
            _authenticationService = authenticationService;
            _unitOfWork = unitOfWork;
            _accountRoleRepository = accountRoleRepository;
            _accountRepository = accountRepository;
        }

        public async Task<Result<AuthenticationResultDto>> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {

            //find user role in db
            List<AccountRole> roles = await _accountRoleRepository.GetRoles();

            //start transaction
            await _unitOfWork.BeginTransactionAsync();
            AuthenticationResultDto authenticationResultDto;
            Account customer;
            if (request.isExternalRegister)
            {
                var registerResult = await _authenticationService.ExternalRegister(cancellationToken);
                if (registerResult.IsSuccess is false)
                {
                    return Result.Fail(registerResult.Errors);
                }
                string identityId = registerResult.Value.identityId;

                customer = Account.CreateBaseCustomer(registerResult.Value.fullName, registerResult.Value.email, identityId, roles);
            }
            else
            {
                //  remember to change to false, for email verification
                var registerResult = await _authenticationService.Register(request.Email, request.Password, request.FullName, true, cancellationToken);
                if (registerResult.IsSuccess is false)
                {
                    return Result.Fail(registerResult.Errors);
                }
                //get identity id from Register.Result()
                string identityId = registerResult.Value;

                customer = Account.CreateBaseCustomer(request.FullName, request.Email, identityId, roles);
            }
            await _accountRepository.Create(customer);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            //var getNewUser = await _accountRepository.GetById( customer.Id);
            if (request.isExternalRegister) 
            {
                authenticationResultDto = (await _authenticationService.ExternalLogin(cancellationToken)).Value;
            }
            else
            {
                authenticationResultDto = (await _authenticationService.Login(request.Email, request.Password, cancellationToken)).Value;
            }
            return Result.Ok(authenticationResultDto);
        }
    }
}

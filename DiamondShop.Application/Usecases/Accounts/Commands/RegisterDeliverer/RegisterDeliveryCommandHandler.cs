using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Accounts.Commands.RegisterStaff;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Accounts.Commands.RegisterDeliverer
{
    public record RegisterDeliveryCommand(string email, string password, FullName fullName) : IRequest<Result<AuthenticationResultDto>>;
    internal class RegisterDeliveryCommandHandler : IRequestHandler<RegisterDeliveryCommand, Result<AuthenticationResultDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;
        private readonly IAuthenticationService _authenticationService;
        public RegisterDeliveryCommandHandler(IUnitOfWork unitOfWork, IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _accountRoleRepository = accountRoleRepository;
            _authenticationService = authenticationService;
        }

        public async Task<Result<AuthenticationResultDto>> Handle(RegisterDeliveryCommand request, CancellationToken cancellationToken)
        {
            List<AccountRole> storeRoles = await _accountRoleRepository.GetRoles();
            //start transaction
            await _unitOfWork.BeginTransactionAsync();
            var identityResult = await _authenticationService.Register(request.email, request.password, request.fullName, true, cancellationToken);
            if (identityResult.IsSuccess is false)
                return Result.Fail(identityResult.Errors);
            string identityId = identityResult.Value;
            Account staff = Account.CreateBaseStaff(request.fullName, request.email, identityId, storeRoles);
            AccountRole? getDeliveryRoll = storeRoles.FirstOrDefault(r => r.Id == AccountRole.Deliverer.Id);
            if(getDeliveryRoll == null)
                return Result.Fail("Delivery role not found");
            staff.AddRole(getDeliveryRoll);
            await _accountRepository.Create(staff);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            var loginResult = await _authenticationService.Login(request.email, request.password, cancellationToken);
            return Result.Ok(loginResult.Value);
        }
    }
}

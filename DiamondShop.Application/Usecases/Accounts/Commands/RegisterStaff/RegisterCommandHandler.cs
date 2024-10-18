using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Accounts.Commands.RegisterStaff
{
    public record RegisterCommand(string email, string password, FullName fullName, bool isManager) : IRequest<Result<AuthenticationResultDto>>;
    internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthenticationResultDto>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IDateTimeProvider _timeProvider;
        private readonly IAccountRoleRepository _accountRoleRepository;

        public RegisterCommandHandler(IAuthenticationService authenticationService, IUnitOfWork unitOfWork, IAccountRepository accountRepository, IDateTimeProvider timeProvider, IAccountRoleRepository accountRoleRepository)
        {
            _authenticationService = authenticationService;
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _timeProvider = timeProvider;
            _accountRoleRepository = accountRoleRepository;
        }

        public async Task<Result<AuthenticationResultDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            List<AccountRole> storeRoles = await _accountRoleRepository.GetRoles();
            //start transaction
            await _unitOfWork.BeginTransactionAsync();
            var identityResult = await _authenticationService.Register(request.email, request.password, request.fullName, true, cancellationToken);
            if (identityResult.IsSuccess is false)
                return Result.Fail(identityResult.Errors);
            string identityId = identityResult.Value;
            Account staff = Account.CreateBaseStaff(request.fullName,request.email,identityId, storeRoles);
            if (request.isManager)
            {
                AccountRole managerRole = storeRoles.First(c => c.Id == AccountRole.Manager.Id);
                staff.AddRole(managerRole);
            }
            await _accountRepository.Create(staff);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            //Account getStaff = await _accountRepository.GetById( staff.Id);
            var loginResult = await _authenticationService.Login(request.email, request.password, cancellationToken);
            return Result.Ok(loginResult.Value);
        }
    }
}

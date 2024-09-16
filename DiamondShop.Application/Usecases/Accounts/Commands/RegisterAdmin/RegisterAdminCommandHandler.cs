using DiamondShop.Application.Services.Data;
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

namespace DiamondShop.Application.Usecases.Accounts.Commands.RegisterAdmin
{
    public record RegisterAdminCommand(string email, string password, FullName fullName) : IRequest<Result<Account>>;
    internal class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, Result<Account>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;

        public RegisterAdminCommandHandler(IAuthenticationService authenticationService, IUnitOfWork unitOfWork, IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository)
        {
            _authenticationService = authenticationService;
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _accountRoleRepository = accountRoleRepository;
        }

        public async Task<Result<Account>> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            List<AccountRole> storeRoles = await _accountRoleRepository.GetRoles();
            //start transaction
            await _unitOfWork.BeginTransactionAsync();
            Account customer;
            var identityResult = await _authenticationService.Register(request.email, request.password, request.fullName, true, cancellationToken);
            if (identityResult.IsSuccess is false)
                return Result.Fail(identityResult.Errors);
            string identityId = identityResult.Value;
            Account staff = Account.CreateAdmin(request.fullName,request.email,identityId,storeRoles);
            await _accountRepository.Create(staff);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            Account getStaff = await _accountRepository.GetById( staff.Id);
            return Result.Ok(getStaff);
            //throw new NotImplementedException();
        }
    }
}

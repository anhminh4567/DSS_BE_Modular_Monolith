using DiamondShop.Application.Services.Data;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.AccountRoleAggregate.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Accounts.Commands.RoleAdd
{
    public record AddRoleCommand(AccountId accId, AccountRoleId roleId) : IRequest<Result>;
    public class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;

        public AddRoleCommandHandler(IUnitOfWork unitOfWork, IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _accountRoleRepository = accountRoleRepository;
        }

        public async Task<Result> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            List<AccountRole> roles = await _accountRoleRepository.GetRoles();
            var getRole = roles.FirstOrDefault(role => role.Id == request.roleId);


            var getAcc = await _accountRepository.GetById(request.accId);
            if(getAcc == null)
                return Result.Fail(new NotFoundError("cannot found such customer role"));

            getAcc.AddRole(getRole);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}

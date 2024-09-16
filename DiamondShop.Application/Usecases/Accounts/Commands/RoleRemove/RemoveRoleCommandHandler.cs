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

namespace DiamondShop.Application.Usecases.Accounts.Commands.RoleRemove
{
    public record RemoveRoleCommand(AccountId accId, AccountRoleId roleId) : IRequest<Result>;
    public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;

        public RemoveRoleCommandHandler(IUnitOfWork unitOfWork, IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _accountRoleRepository = accountRoleRepository;
        }

        public async Task<Result> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
        {
            var getAcc = await _accountRepository.GetById(request.accId);
            if (getAcc == null)
                return Result.Fail(new NotFoundError("cannot found user with such id"));
            AccountRole? userRoleToRemove = getAcc.Roles.FirstOrDefault(r => r.Id == request.roleId);
            if (userRoleToRemove == null)
                return Result.Fail(new NotFoundError("cannot found such role in user"));
            getAcc.RemoveRole(userRoleToRemove);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}

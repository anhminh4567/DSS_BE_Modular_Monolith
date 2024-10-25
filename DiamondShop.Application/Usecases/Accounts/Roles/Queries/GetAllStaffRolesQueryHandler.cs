using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Accounts.Roles.Queries
{
    public record GetAllStaffRolesQuery() : IRequest<List<AccountRole>>;
    internal class GetAllStaffRolesQueryHandler : IRequestHandler<GetAllStaffRolesQuery, List<AccountRole>>
    {
        private readonly IAccountRoleRepository _accountRoleRepository;

        public GetAllStaffRolesQueryHandler(IAccountRoleRepository accountRoleRepository)
        {
            _accountRoleRepository = accountRoleRepository;
        }
        public async Task<List<AccountRole>> Handle(GetAllStaffRolesQuery request, CancellationToken cancellationToken)
        {
            var result = await _accountRoleRepository.GetAll();
            return result.Where(r => r.RoleType == Domain.Models.AccountRoleAggregate.AccountRoleType.Staff).ToList();
        }
    }

}

using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using MediatR;

namespace DiamondShop.Application.Usecases.Accounts.Roles.Queries
{
    public record GetAllUserRolesQuery() : IRequest<List<AccountRole>>;
    internal class GetAllUserRolesQueryHandler : IRequestHandler<GetAllUserRolesQuery, List<AccountRole>>
    {
        private readonly IAccountRoleRepository _accountRoleRepository;

        public GetAllUserRolesQueryHandler(IAccountRoleRepository accountRoleRepository)
        {
            _accountRoleRepository = accountRoleRepository;
        }

        public async Task<List<AccountRole>> Handle(GetAllUserRolesQuery request, CancellationToken cancellationToken)
        {
            var result = await _accountRoleRepository.GetAll();
            return result.Where(r => r.RoleType == Domain.Models.AccountRoleAggregate.AccountRoleType.Customer).ToList();
        }
    }
}

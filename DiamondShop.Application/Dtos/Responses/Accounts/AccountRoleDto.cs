using DiamondShop.Domain.Models.AccountRoleAggregate;

namespace DiamondShop.Application.Dtos.Responses.Accounts
{
    public class AccountRoleDto
    {
        public string Id { get; set; }
        public string RoleName { get;  set; }
        public AccountRoleType RoleType { get;  set; }
        public string? RoleDescription { get;  set; }
    }
}

using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.RoleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Accounts
{
    public class AccountDto
    {
        public string Id { get; set; }
        public string IdentityId { get;  set; }
        public List<AccountRoleDto> Roles { get;  set; } = new();
        public List<AddressDto> Addresses { get;  set; } = new();
        public string FirstName { get;  set; }
        public string LastName { get;  set; }
        public decimal? TotalPoint { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get;  set; }
        public IUserIdentity? UserIdentity { get; set; }
    }
}

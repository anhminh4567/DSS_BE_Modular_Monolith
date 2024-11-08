using DiamondShop.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Accounts
{
    public class AccountMinimalDto
    {
        public List<AccountRoleDto> Roles { get; set; } = new();
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

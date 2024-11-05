using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.RoleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IAccountServices
    {
        decimal GetUserPointFromOrderComplete(Account userAccount, Order completedOrder);

        Task CheckAndUpdateUserRankIfQualified(List<Account> userAccountToCheck, List<AccountRole> allUserRoles, AccountRules accountRules);
    }
}

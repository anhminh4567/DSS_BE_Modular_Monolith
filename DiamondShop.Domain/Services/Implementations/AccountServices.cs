using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    public class AccountServices : IAccountServices
    {
        public AccountServices()
        {
        }
        public decimal GetUserPointFromOrderComplete(Account userAccount, Order completedOrder)
        {
            return GetUserPointFromOrderCompleteGlobal(userAccount, completedOrder);
        }
        public static decimal GetUserPointFromOrderCompleteGlobal(Account userAccount, Order completedOrder)
        {
            var orderTotal = completedOrder.TotalPrice;
            var convertedToPoint = AccountRules.OrderPriceToPoint(orderTotal);
            return convertedToPoint;
        }
        public async Task CheckAndUpdateUserRankIfQualified(List<Account> userAccountToCheck, List<AccountRole> allUserRoles, AccountRules accountRules)
        {
             await CheckAndUpdateUserRankIfQualifiedGlobal(userAccountToCheck, allUserRoles, accountRules);
        }
        public static async Task CheckAndUpdateUserRankIfQualifiedGlobal(List<Account> userAccountToCheck, List<AccountRole> allUserRoles, AccountRules accountRules)
        {
            var goldRank = allUserRoles.FirstOrDefault(x => x.Id == AccountRole.CustomerGold.Id);
            var silverRank = allUserRoles.FirstOrDefault(x => x.Id == AccountRole.CustomerSilver.Id);
            var bronzeRank = allUserRoles.FirstOrDefault(x => x.Id == AccountRole.CustomerBronze.Id);
            var option = accountRules;
            foreach (var acc in userAccountToCheck)
            {
                var userRoles = acc.Roles;
                var userPoint = acc.TotalPoint;
                if (IsGoldRankQualified(acc, option, userRoles)) 
                {
                    userRoles.Add(goldRank);
                    var getSilver = userRoles.FirstOrDefault(x => x.Id == silverRank.Id);
                    var getBronze = userRoles.FirstOrDefault(x => x.Id == bronzeRank.Id);
                    if(getSilver != null)
                        userRoles.Remove(getSilver);
                    if (getBronze != null)
                        userRoles.Remove(getBronze);
                    return;
                }


                if (IsSilverRankQualified(acc, option, userRoles)) 
                {
                    userRoles.Add(silverRank);
                    var getGold = userRoles.FirstOrDefault(x => x.Id == goldRank.Id);
                    var getBronze = userRoles.FirstOrDefault(x => x.Id == bronzeRank.Id);
                    if(getGold != null  )
                        userRoles.Remove(getGold);
                    if (getBronze != null)
                        userRoles.Remove(getBronze);
                    return;
                }


                if (IsBronzeRankQualified(acc, option, userRoles))
                {
                    userRoles.Add(bronzeRank);
                    var getgold = userRoles.FirstOrDefault(x => x.Id == goldRank.Id);
                    var getsilver = userRoles.FirstOrDefault(x => x.Id == silverRank.Id);
                    if (getgold != null)
                        userRoles.Remove(getgold);
                    if (getsilver != null)
                        userRoles.Remove(getsilver);
                    return;
                }
            }
        }
        private static bool IsGoldRankQualified(Account user, AccountRules options, List<AccountRole> roles)
        {
            if (roles.Any(x => x.Id == AccountRole.CustomerGold.Id)) // already in rank
                return false;
            var goldRank = options.TotalPointToGold;
            if (user.TotalPoint >= goldRank)
                return true;
            return false;
        }
        private static bool IsSilverRankQualified(Account user, AccountRules options, List<AccountRole> roles)
        {
            if (roles.Any(x => x.Id == AccountRole.CustomerSilver.Id)) // already in rank
                return false;
            var silverRank = options.TotalPointToSilver;
            if (user.TotalPoint >= silverRank)
                return true;
            return false;
        }
        private static bool IsBronzeRankQualified(Account user, AccountRules options, List<AccountRole> roles)
        {
            if (roles.Any(x => x.Id == AccountRole.CustomerBronze.Id)) // already in rank
                return false;
            var bronzeRank = options.TotalPointToBronze;
            if (user.TotalPoint >= bronzeRank)
                return true;
            return false;
        }
    }
}

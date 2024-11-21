using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.Domain
{
    public class AccountServiceTest
    {
        private static List<AccountRole> CustomerRoles = new List<AccountRole>(AccountRole.CustomerRoles);
        private static AccountRules accountRules = new AccountRules();
        private static AccountRole customerBaseRole = CustomerRoles.First(x => x.Id == AccountRole.Customer.Id);
        private static AccountRole customerBronzeRole = CustomerRoles.First(x => x.Id == AccountRole.CustomerBronze.Id);
        private static AccountRole customerSilverRole = CustomerRoles.First(x => x.Id == AccountRole.CustomerSilver.Id);
        private static AccountRole customerGoldRole = CustomerRoles.First(x => x.Id == AccountRole.CustomerGold.Id);
        [Fact]
        public void CheckIfBronzeRankMatched_Should_Add_Bronze()
        {
            //arrange
            var bronzeRank = CustomerRoles.First(x => x.Id == AccountRole.CustomerBronze.Id);
            var account = Account.Create(FullName.Create("adfs","asdf"),"asdf@gmail.com");
            account.AddRole(CustomerRoles.First(x => x.Id == AccountRole.Customer.Id));
            var bronzePoint = accountRules.TotalPointToBronze;
            account.TotalPoint = bronzePoint;
            //act
            AccountServices.CheckAndUpdateUserRankIfQualifiedGlobal(new List<Account>() { account }, CustomerRoles, accountRules).Wait();

            //assert
            account.Roles.Exists(x => x.Id == bronzeRank.Id);
            account.Roles.Count.Equals(2);
        }
        [Fact]
        public void CheckIfSilverRankMatched_Should_Add_Silver()
        {
            //arrange
            var silverRank = CustomerRoles.First(x => x.Id == AccountRole.CustomerSilver.Id);
            var account = Account.Create(FullName.Create("adfs", "asdf"), "asdf@gmail.com");
            account.AddRole(CustomerRoles.First(x => x.Id == AccountRole.Customer.Id));
            account.AddRole(CustomerRoles.First(x => x.Id == AccountRole.CustomerBronze.Id));
            var silverPoint = accountRules.TotalPointToSilver;
            account.TotalPoint = silverPoint;
            //act
            AccountServices.CheckAndUpdateUserRankIfQualifiedGlobal(new List<Account>() { account }, CustomerRoles, accountRules).Wait();

            //assert
            account.Roles.Exists(x => x.Id == silverRank.Id);
            account.Roles.Exists(x => x.Id == customerBaseRole.Id);
            account.Roles.Count.Equals(2);
            Assert.DoesNotContain( account.Roles, r => r.Id == customerBronzeRole.Id);
            Assert.DoesNotContain(account.Roles, r => r.Id == customerGoldRole.Id);
        }
        [Fact]
        public void CheckIfGoldRankMatched_Should_Add_Gold()
        {
            //arrange
            var goldRank = CustomerRoles.First(x => x.Id == AccountRole.CustomerGold.Id);
            var account = Account.Create(FullName.Create("adfs", "asdf"), "asdf@gmail.com");
            account.AddRole(CustomerRoles.First(x => x.Id == AccountRole.Customer.Id));
            account.AddRole(CustomerRoles.First(x => x.Id == AccountRole.CustomerSilver.Id));
            var goldPoint = accountRules.TotalPointToGold;
            account.TotalPoint = goldPoint;
            //act
            AccountServices.CheckAndUpdateUserRankIfQualifiedGlobal(new List<Account>() { account }, CustomerRoles, accountRules).Wait();

            //assert
            account.Roles.Exists(x => x.Id == goldRank.Id);
            account.Roles.Exists(x => x.Id == customerBaseRole.Id);
            account.Roles.Count.Equals(2);
            Assert.DoesNotContain(account.Roles, r => r.Id == customerBronzeRole.Id);
            Assert.DoesNotContain(account.Roles, r => r.Id == customerSilverRole.Id);
        }
    }
}

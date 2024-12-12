using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Orders;
using System.Runtime.CompilerServices;
using DiamondShop.Domain.Models.AccountAggregate.Events;
using System.ComponentModel.DataAnnotations.Schema;
using DiamondShop.Domain.Models.AccountAggregate.Enums;

namespace DiamondShop.Domain.Models.AccountAggregate
{
    public class Account : Entity<AccountId> , IAggregateRoot
    {
        public static Account AnonymousCustomer => new Account() 
        {
            Id = AccountId.Parse("-1"),
            FullName = new FullName("Anonymous", "Customer"),
            Email = "anonymousecustomer@notreal",
            IdentityId = "NONE",
            TotalPoint = -1,
            UserIdentity = null,
            Roles = new List<AccountRole> { AccountRole.Customer }
        };
        public string IdentityId { get; private set; }
        public List<AccountRole> Roles { get; private set; } = new();
        public List<Address> Addresses { get; private set; } = new();
        public FullName FullName { get; private set; }
        public string Email { get; private set; }
        public string? PhoneNumber { get; set; }
        public decimal TotalPoint { get; set; } = 0;
        public AccountStatus  Status { get; set; } = AccountStatus.Active;
        [NotMapped]
        public IUserIdentity? UserIdentity { get; set; }
        [NotMapped]
        public List<Order>? CustomerOrders { get; set; } = new();
        //public List<Blog> Blogs { get; private set; }
        //public List<JewelryReview> JewelryReviews { get; private set; }
        //public List<Order> Orders { get; private set; }

        private Account(AccountId accountId, FullName fullName, string email) : base(accountId)
        {
            FullName = fullName;
            Email = email;
        }
        public static Account Create(FullName fullName, string email)
        {
            var user = new Account(AccountId.Create(), fullName, email);
            return user;
        }
        public static Account CreateBaseCustomer(FullName fullName, string email, string identityId, List<AccountRole> allRoles)
        {
            var user = Create(fullName,email);
            user.SetIdentity(identityId);
            AccountRole roleToAdd = allRoles.First(r => r.Id == AccountRole.Customer.Id);

            user.AddRole(roleToAdd);
            //add domain events
            user.Raise(new CustomerCreatedMessage(user.Id, DateTime.UtcNow));
            return user;
        }
        public static Account CreateBaseStaff(FullName fullName, string email, string identityId, List<AccountRole> allRoles)
        {
            var user = Create(fullName, email);
            user.SetIdentity(identityId);
            AccountRole roleToAdd = allRoles.First(r => r.Id == AccountRole.Staff.Id);

            user.AddRole(roleToAdd);
            return user;
        }
        public static Account CreateBaseManager(FullName fullName, string email, string identityId, List<AccountRole> allRoles)
        {
            var user = Create(fullName, email);
            user.SetIdentity(identityId);
            AccountRole roleToAdd = allRoles.First(r => r.Id == AccountRole.Staff.Id);
            user.AddRole(roleToAdd);
            roleToAdd = allRoles.First(r => r.Id == AccountRole.Manager.Id);
            user.AddRole(roleToAdd);
            return user;
        }
        public static Account CreateBaseDeliverer(FullName fullName, string email, string identityId, List<AccountRole> allRoles)
        {
            var user = Create(fullName, email);
            user.SetIdentity(identityId);
            AccountRole roleToAdd = allRoles.First(r => r.Id == AccountRole.Deliverer.Id);
            user.AddRole(roleToAdd);
            return user;
        }
        public static Account CreateAdmin(FullName fullName, string email, string identityId, List<AccountRole> allRoles)
        {
            var user = Create(fullName, email);
            user.SetIdentity(identityId);
            AccountRole roleToAdd_staff = allRoles.First(r => r.Id == AccountRole.Staff.Id);
            AccountRole roleToAdd_Admin = allRoles.First(r => r.Id == AccountRole.Admin.Id);
            user.AddRole(roleToAdd_staff);
            user.AddRole(roleToAdd_Admin);

            return user;
        }
        public void SetIdentity(string identityID)
        {
            IdentityId = identityID;
        }
        public void AddRole(AccountRole role)
        {
            ArgumentNullException.ThrowIfNull(role);
            Roles.Add(role);
        }
        public void RemoveRole(AccountRole role)
        {
            ArgumentNullException.ThrowIfNull(role);
            Roles.Remove(role);
        }
        public void AddAddress(int provinceId,string province, string district, string ward, string street)
        {
            Addresses.Add(new Address(province.Trim(), district.Trim(), ward.Trim(), street.Trim(), Id) { ProvinceId = provinceId});
        }
        public void RemoveAddress(AddressId addressId)
        {
            var get = Addresses.FirstOrDefault(a => a.Id == addressId);
            if(get != null)
                Addresses.Remove(get);
        
        }
        public void ChangeFullName(FullName fullName)
        {
            if(fullName is null || fullName.FirstName is null || fullName.LastName is null)
            {
                throw new ArgumentNullException("Fullname is invalid");
            }
            FullName = fullName;
        }
        public void AddTotalPoint(decimal amount)
        {
            TotalPoint += amount;
            if (TotalPoint < 0)
                TotalPoint = 0;
        }
        
        private Account()
        {

        }
        public void SetStatus(AccountStatus status)
        {
            Status = status;
        }
    }
}

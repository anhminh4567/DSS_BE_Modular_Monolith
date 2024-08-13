using BeatvisionRemake.Domain.Common;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.CustomerAggregate.ValueObjects;
using DiamondShop.Domain.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.CustomerAggregate
{
    public class Customer : Entity<CustomerId>, IAggregateRoot
    {
        private Customer()
        {

        }
        private Customer(CustomerId customerId, IUserIdentity identityId, FullName fullName, DateTime? birthDate = null) : base(customerId)
        {
            Identity = identityId;
            FullName = fullName;
            BirthDate = birthDate;
        }
        public string IdentityId { get; private set; }
        public IUserIdentity Identity { get; private set; }
        public FullName FullName { get; private set; }
        public DateTime? BirthDate { get; private set; }
        public string Email { get; private set; }
        public static Customer Create(IUserIdentity identityId, FullName fullName)
        {
            var user = new Customer(CustomerId.Create(), identityId, fullName);
            user.SetIdentity(identityId);
            return user;
        }
        private void SetIdentity(IUserIdentity userIdentity)
        {
            IdentityId = userIdentity.IdentityId;
        }
    }
}

using DiamondShop.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Roles
{
    public class DiamonShopRole
    {
        public Role Value { get; private set; }
        public DiamonShopRole(Role value)
        {
            Value = value;
        }
        public static implicit operator Role(DiamonShopRole diamondShopRole)
        {
            return diamondShopRole.Value;
        }
    }
    public class DiamondShopStoreRoles : DiamonShopRole
    {
        public static DiamondShopStoreRoles Staff = new DiamondShopStoreRoles(new Role("2","staff"));
        public static DiamondShopStoreRoles Manager = new DiamondShopStoreRoles( new Role("3","manager"));
        public static DiamondShopStoreRoles StorageManager = new  DiamondShopStoreRoles (new Role("4","storagemanager"));
        public DiamondShopStoreRoles(Role value) : base(value)
        {
        }
    }
    public class DiamondShopCustomerRole : DiamonShopRole
    {

        public static DiamondShopCustomerRole Customer = new  DiamondShopCustomerRole( new Role("1", "customer"));
        public DiamondShopCustomerRole(Role value) : base(value) { }
    }
    public class DiamondShopAdminRole : DiamonShopRole
    {
        public static DiamondShopAdminRole Admin = new DiamondShopAdminRole( new Role("5", "admin"));

        public DiamondShopAdminRole(Role value) : base(value)
        {
        }
    }

}

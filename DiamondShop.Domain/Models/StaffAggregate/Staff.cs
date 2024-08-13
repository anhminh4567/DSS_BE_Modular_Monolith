using BeatvisionRemake.Domain.Common;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.StaffAggregate.ValueObjects;
using DiamondShop.Domain.Roles;

namespace DiamondShop.Domain.Models.StaffAggregate
{
    public class Staff : Entity<StaffId>
    {
        private Staff()
        {

        }
        public Staff(StaffId staffId,IUserIdentity identity, FullName fullName, string departmentLocation) : base(staffId)
        {
            Identity = identity;
            FullName = fullName;
            DepartmentLocation = departmentLocation;
        }
        public string IdentityId { get; private set; }
        public IUserIdentity Identity { get; private set; }
        public FullName FullName { get; private set; }
        public string DepartmentLocation { get; private set; }
        public static Staff Create(IUserIdentity identity, FullName fullName,string departmentLocation)
        {
            var user = new Staff(StaffId.Create(),identity,fullName,departmentLocation);
            user.SetIdentity(identity);
            return user;
        }
        private void SetIdentity(IUserIdentity userIdentity)
        {
            IdentityId = userIdentity.IdentityId;

        }
    }
}

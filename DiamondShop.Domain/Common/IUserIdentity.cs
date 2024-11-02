using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Common
{
    public interface IUserIdentity
    {
        public string IdentityId { get; }
        public string Email { get; }
        public bool IsBan { get; }
        public bool IsEmailConfirmed { get; }
        public DateTime? BanEndDate { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Options
{
    internal class AuthenticationRestrictionOption
    {
        public const string Section = "AuthenticationRestriction";
        public int LockoutMiniute {  get; set; }
        public int AllowedRetry { get; set; }
        public int TokenLifeTimeMinute { get; set; }


    }
}

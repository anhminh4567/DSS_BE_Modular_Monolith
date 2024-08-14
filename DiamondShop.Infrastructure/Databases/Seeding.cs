using DiamondShop.Infrastructure.Databases.Configurations;
using DiamondShop.Infrastructure.Securities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases
{
    public class Seeding
    {
        public static async Task SeedAsync(CustomUserManager userManager, CustomRoleManager roleManager )
        {
            var normalizer = new UpperInvariantLookupNormalizer();
            foreach (var role in IdentityConfiguration.SYSTEM_ROLE)
            {
                if(await roleManager.RoleExistsAsync(role.Name))
                {
                    continue;
                }
                await roleManager.CreateAsync(role);
            }
               
        }
    }
}

using DiamondShop.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Securities
{
    internal class CustomSigninManager : SignInManager<CustomIdentityUser>
    {
        public CustomSigninManager(UserManager<CustomIdentityUser> userManager,
            IHttpContextAccessor contextAccessor, 
            IUserClaimsPrincipalFactory<CustomIdentityUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<CustomIdentityUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<CustomIdentityUser> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }
    }
}

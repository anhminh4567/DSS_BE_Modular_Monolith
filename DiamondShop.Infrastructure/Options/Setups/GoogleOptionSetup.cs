using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Options.Setups
{
    internal class GoogleOptionSetup : IConfigureNamedOptions<GoogleOptions>
    {
        private readonly IOptions<ExternalAuthenticationOptions> _options;

        public GoogleOptionSetup(IOptions<ExternalAuthenticationOptions> options)
        {
            _options = options;
        }

        public void Configure(string? name, GoogleOptions options)
        {
            Configure(options);
        }

        public void Configure(GoogleOptions options)
        {
            var googleOption = _options.Value.GoogleAuthenticationOption;
            options.ClientId = googleOption.ClientId;
            options.ClientSecret = googleOption.ClientSecret;
            options.AuthorizationEndpoint = GoogleDefaults.AuthorizationEndpoint;
            options.UserInformationEndpoint = GoogleDefaults.UserInformationEndpoint;
            options.TokenEndpoint = GoogleDefaults.TokenEndpoint;
            options.SignInScheme = IdentityConstants.ExternalScheme;
            options.CallbackPath = "/signin-google";
            options.AccessType = "offline";
            options.SaveTokens = true;
            //opt.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
            //opt.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Events.OnCreatingTicket = async (ticketContext) =>
            {
                var oauthProp = ticketContext.Properties;
                var identity = ticketContext.Identity;
                var items = oauthProp.Items;
                var accessToken = ticketContext.AccessToken;//oauthProp.Ticket.Properties.GetTokenValue("access_token");
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
                if (response.IsSuccessStatusCode is false)
                {
                    identity.AddClaim(new System.Security.Claims.Claim(ExternalAuthenticationOptions.EXTERNAL_FAILURE_CLAIM_NAME, "FAILURE"));
                }
                else
                {
                    var readResult = JsonConvert.DeserializeObject<GoogleOauthInfo>(await response.Content.ReadAsStringAsync());
                    identity.AddClaim(new System.Security.Claims.Claim(ExternalAuthenticationOptions.EXTERNAL_USERNAME_CLAIM_NAME, readResult.Name));
                    identity.AddClaim(new System.Security.Claims.Claim(ExternalAuthenticationOptions.EXTERNAL_EMAIL_CLAIM_NAME, readResult.Email));
                    identity.AddClaim(new System.Security.Claims.Claim(ExternalAuthenticationOptions.EXTERNAL_PROFILE_IMAGE_CLAIM_NAME, readResult.Picture));
                }
            };
        }
        private class GoogleOauthInfo
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string Picture { get; set; }
        }
    }
    
}

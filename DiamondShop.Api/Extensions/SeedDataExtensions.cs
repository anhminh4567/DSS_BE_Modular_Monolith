using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Securities;
using System.Net.NetworkInformation;

namespace DiamondShop.Api.Extensions
{
    internal static class SeedDataExtensions
    {
        public static async void SeedData(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            CustomUserManager userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager>();
            CustomRoleManager roleManager = scope.ServiceProvider.GetRequiredService<CustomRoleManager>();
            CustomSigninManager signinManager = scope.ServiceProvider.GetRequiredService<CustomSigninManager>();
            await Seeding.SeedAsync(userManager,roleManager);
        }
    }
}

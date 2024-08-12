
using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<IdentityId> Register(string email, string password, string fullname, CancellationToken cancellationToken = default);
        Task<AuthenticationResultDto> Login(string email, string password, CancellationToken cancellationToken = default);
        //Task<bool> Logout();
        Task GetUserIdentity();
        Task AddToRole();
        Task RemoveFromRole();
        Task Ban(TimeSpan time);
        Task ConfirmEmail();
        Task SendConfirmEmail();
        Task ResetPassword();
        Task GenerateResetPasswordToken();

    }
}

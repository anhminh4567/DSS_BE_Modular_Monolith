using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.Accounts;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Accounts.Commands.BanAccount;
using DiamondShop.Application.Usecases.Accounts.Commands.Login;
using DiamondShop.Application.Usecases.Accounts.Commands.RefreshingToken;
using DiamondShop.Application.Usecases.Accounts.Commands.RegisterAdmin;
using DiamondShop.Application.Usecases.Accounts.Commands.RegisterCustomer;
using DiamondShop.Application.Usecases.Accounts.Commands.RegisterDeliverer;
using DiamondShop.Application.Usecases.Accounts.Commands.RegisterStaff;
using DiamondShop.Application.Usecases.Accounts.Commands.RoleAdd;
using DiamondShop.Application.Usecases.Accounts.Commands.RoleRemove;
using DiamondShop.Application.Usecases.Accounts.Queries.GetCounts;
using DiamondShop.Application.Usecases.Accounts.Queries.GetDelivererStatus;
using DiamondShop.Application.Usecases.Accounts.Queries.GetDetail;
using DiamondShop.Application.Usecases.Accounts.Queries.GetPaging;
using DiamondShop.Application.Usecases.Accounts.Roles.Queries;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Infrastructure.Options;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace DiamondShop.Api.Controllers.Accounts
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly IAuthenticationService _authenticationService;
        private readonly IOptions<FrontendOptions> _options;

        public AccountController(ISender sender, IMapper mapper, IAuthenticationService authenticationService, IOptions<FrontendOptions> options)
        {
            _sender = sender;
            _mapper = mapper;
            _authenticationService = authenticationService;
            _options = options;
        }

        [HttpPost("Register")]
        [Consumes("application/json")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResultDto), 200)]
        [ProducesResponseType(301)]

        public async Task<ActionResult> Register([FromBody] RegisterCustomerCommand registerCustomerCommand, CancellationToken cancellationToken = default)
        {
            var command = registerCustomerCommand with { isExternalRegister = false };
            var result = await _sender.Send(command);
            if (result.IsSuccess is false)
                return MatchError(result.Errors, ModelState);
            return Ok(result.Value);

        }
        [HttpGet("Register/External")]
        [AllowAnonymous]
        [ProducesResponseType(301)]
        public async Task<ActionResult> ExternalRegister([FromQuery] string? externalProviderName, CancellationToken cancellationToken = default)
        {
            var result = await _authenticationService.GetProviderAuthProperty(externalProviderName, Url.Action(nameof(ExternalRegisterCallback)));
            if (result.IsSuccess is false)
            {
                return MatchError(result.Errors, ModelState);
            }
            Microsoft.AspNetCore.Authentication.AuthenticationProperties authenticationProperties = result.Value;
            return new ChallengeResult(externalProviderName, authenticationProperties);
        }
        [HttpPost("Login")]
        [Consumes("application/json")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResultDto), 200)]
        [ProducesResponseType(301)]
        public async Task<ActionResult> Login([FromBody] LoginCommand? loginCommand, [FromQuery] string? externalProviderName, CancellationToken cancellationToken = default)
        {
            if (externalProviderName == null && loginCommand is not null)
            {
                var command = loginCommand with { isExternalLogin = false };
                var result = await _sender.Send(command, cancellationToken);
                if (result.IsSuccess is false)
                    return MatchError(result.Errors, ModelState);
                return Ok(result.Value);
            }
            else
            {
                var result = await _authenticationService.GetProviderAuthProperty(externalProviderName, Url.Action(nameof(ExternalLoginCallback)));
                Microsoft.AspNetCore.Authentication.AuthenticationProperties authenticationProperties = result.Value;
                return new ChallengeResult(externalProviderName, authenticationProperties);
            }
        }
        [HttpPost("LoginStaff")]
        [Consumes("application/json")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResultDto), 200)]

        public async Task<ActionResult> LoginStaff([FromBody] LoginCommand loginCommand, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(loginCommand, cancellationToken);
            if (result.IsSuccess is false)
                return MatchError(result.Errors, ModelState);
            return Ok(result.Value);
        }
        [HttpPost("RegisterStaff")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(AuthenticationResultDto), 200)]
        public async Task<ActionResult> RegisterStaff([FromBody] RegisterCommand registerCommand, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(registerCommand, cancellationToken);
            if (result.IsSuccess is false)
                return MatchError(result.Errors, ModelState);
            return Ok(result.Value);
        }
        [HttpPost("RegisterDeliverer")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(AuthenticationResultDto), 200)]
        public async Task<ActionResult> RegisterDeliverer([FromBody] RegisterDeliveryCommand registerDeliveryCommand, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(registerDeliveryCommand, cancellationToken);
            if (result.IsSuccess is false)
                return MatchError(result.Errors, ModelState);
            return Ok(result.Value);
        }
        [HttpPost("RegisterAdmin")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(AuthenticationResultDto), 200)]
        public async Task<ActionResult> RegisterAdmin([FromBody] RegisterAdminCommand registerAdminCommand, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(registerAdminCommand, cancellationToken);
            if (result.IsSuccess is false)
                return MatchError(result.Errors, ModelState);
            return Ok(result.Value);
        }
        [HttpGet("external-register-callback-url")]
        public async Task<ActionResult> ExternalRegisterCallback()
        {
            //this command is mock up for it to pass the validator
            //this wont play to anything
            var result = await _sender.Send(new RegisterCustomerCommand("fake@gmail.com", "123", FullName.Create("abc", "abc"), isExternalRegister: true));
            if (result.IsSuccess is false)
                return MatchError(result.Errors, ModelState);
            return await ExternalLoginCallback();
            //return Redirect("https://diamondshop-app.vercel.app");
            //return Ok(result.Value);
        }
        [HttpGet("external-Login-callback-url")]
        public async Task<ActionResult> ExternalLoginCallback()
        {
            //this command is mock up for it to pass the validator
            var result = await _sender.Send(new LoginCommand("123132", "123123", true));
            if (result.IsSuccess is false)
                return MatchError(result.Errors, ModelState);
            // string accessToken, DateTime expiredAccess, string refreshToken, DateTime expiredRefresh
            var value = result.Value;
            var url = $"{_options.Value.BaseUrl}?accessToken={value.accessToken}&expiredAccess={value.expiredAccess}&refreshToken=" +
                $"{value.refreshToken}&expiredRefresh={value.expiredRefresh}";
            return Redirect(url);
            //return Ok(result.Value);
        }

        [HttpPut("RefreshToken")]
        [Authorize]
        [ProducesResponseType(typeof(AuthenticationResultDto), 200)]
        public async Task<ActionResult> RefreshingToken([FromQuery] string refreshToken, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new RefreshingTokenCommand(refreshToken), cancellationToken);
            if (result.IsSuccess is false)
                return MatchError(result.Errors, ModelState);
            return Ok(result.Value);
        }
        [HttpPut("Ban")]
        [Authorize(Roles = AccountRole.AdminId)]
        public async Task<ActionResult> BanAccount([FromQuery] string identityId, [FromQuery] string? banEndDate, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new BanAccountCommand(identityId,banEndDate), cancellationToken);
            if (result.IsSuccess is false)
                return MatchError(result.Errors, ModelState);
            return Ok();
        }

        [HttpPut("AddRole")]
        public async Task<ActionResult> UpRank([FromBody] AddRoleCommand upRankCustomerCommand, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(upRankCustomerCommand, cancellationToken);
            if (result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("RemoveRole")]
        public async Task<ActionResult> DownRank([FromBody] RemoveRoleCommand downRankCustomerCommand, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(downRankCustomerCommand, cancellationToken);
            if (result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }

        [HttpGet("Paging")]
        [ProducesResponseType(typeof(PagingResponseDto<AccountDto>), 200)]
        public async Task<ActionResult> GetPaging([FromQuery] GetAccountPagingQuery getCustomerPageQuery)
        {
            var result = await _sender.Send(getCustomerPageQuery);
            var mappedResult = _mapper.Map<PagingResponseDto<AccountDto>>(result.Value);
            return Ok(mappedResult);
        }


        [HttpGet("{accountId}")]
        [ProducesResponseType(typeof(AccountDto), 200)]
        //[Authorize]
        public async Task<ActionResult> GetDetail([FromRoute] string accountId, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAccountDetailQuery(AccountId.Parse(accountId)), cancellationToken);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<AccountDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpGet("Role/Staff")]
        [ProducesResponseType(typeof(List<AccountRoleDto>), 200)]
        //[Authorize]
        public async Task<ActionResult> GetStaffRoles(CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAllStaffRolesQuery(), cancellationToken);
            var mappedResult = _mapper.Map<List<AccountRoleDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("Role/User")]
        [ProducesResponseType(typeof(List<AccountRoleDto>), 200)]
        //[Authorize]
        public async Task<ActionResult> GetUserRoles(CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAllUserRolesQuery(), cancellationToken);
            var mappedResult = _mapper.Map<List<AccountRoleDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("Email/Confirm")]
        public async Task<ActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string userId)
        {
            var result = await _authenticationService.ConfirmEmail(userId, token);
            if (result.IsFailed)
            {
                var url = _options.Value.ConfirmEmailFailedUrl + "?errorMessage=" + result.Errors.First().Message;
                return Redirect(url);
            }
            return Redirect(_options.Value.ConfirmEmailSuccessUrl + "?userId=" + userId + "&token=" + token);
        }
        [HttpGet("{accountId}/Email/SendConfirm")]
        public async Task<ActionResult> SendConfirmEmail([FromRoute] string accountId)
        {
            var result = await _authenticationService.SendConfirmEmail(accountId);
            if(result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }


        [HttpGet("DelivererStatus/All")]
        [Produces(typeof(List<DelivererStatusDto>))]
        public async Task<ActionResult> GetAllDelivereAndStatus()
        {
            var result = await _sender.Send(new GetDeliveresAndTheirStatusQuery());
            return Ok(result);
        }
        [HttpPost("Google/Credential")]
        [Produces(typeof(AuthenticationResultDto))]
        public async Task<ActionResult> GoogleTokenFromFrontend([FromBody]string credential)
        {
            var result = await _authenticationService.GoogleHandler(credential);
            if(result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors,ModelState);
        }
        [HttpDelete("{identityId}")]
        public async Task<ActionResult> DeleteByIdentity([FromRoute] string identityId)
        {
            var result = await _authenticationService.DeleteByIdentityUser(identityId);
            if(result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("{identityId}/ResetPassword")]
        public async Task<ActionResult> ChangePassword([FromRoute] string identityId, [FromForm] string oldPassword, [FromForm] string newPassword)
        {
            var result = await _authenticationService.ChangePassword(identityId,oldPassword,newPassword);
            if (result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("{identityId}/ForgetPassword")]
        public async Task<ActionResult> ResetPassword([FromRoute] string identityId, [FromForm] string newPassword)
        {
            var result = await _authenticationService.ResetPassword(identityId, newPassword);
            if (result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Customers.Commands.RegisterCustomer;
using DiamondShop.Application.Usecases.Customers.Queries.LoginCustomer;
using DiamondShop.Domain.Common.ValueObjects;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(ISender sender, IAuthenticationService authenticationService)
        {
            _sender = sender;
            _authenticationService = authenticationService;
        }

        [HttpPost("Register")]
        [Consumes("application/json")]
        public async Task<ActionResult> Register([FromBody]RegisterCustomerCommand? registerCustomerCommand, [FromQuery] string? externalProviderName, CancellationToken cancellationToken = default)
        {
            if(externalProviderName == null && registerCustomerCommand is not null) 
            {
                var command = registerCustomerCommand with { isExternalRegister = false };
                var result = await _sender.Send(command);
                if (result.IsSuccess is false)
                    return MatchError(result.Errors, ModelState);
                return Ok(result.Value);
            }
            else
            {
                var result = await _authenticationService.GetProviderAuthProperty(externalProviderName,Url.Action(nameof(ExternalRegisterCallback)));
                if(result.IsSuccess is false)
                {
                    return MatchError(result.Errors, ModelState);
                }
                Microsoft.AspNetCore.Authentication.AuthenticationProperties authenticationProperties = result.Value;
                return new ChallengeResult(externalProviderName,authenticationProperties);
            }

        }
        [HttpPost("Login")]
        [Consumes("application/json")]
        public async Task<ActionResult> Login([FromBody] LoginCustomerQuery? loginCustomerQuery , [FromQuery] string? externalProviderName, CancellationToken cancellationToken = default)
        {
            if(externalProviderName == null && loginCustomerQuery is not null) 
            {
                var command = loginCustomerQuery with { isExternalLogin = false };
                var result = await _sender.Send(command);
                if (result.IsSuccess is false)
                    return MatchError(result.Errors, ModelState);
                return Ok(result.Value);
            }
            else
            {
                var result = await _authenticationService.GetProviderAuthProperty(externalProviderName, Url.Action(nameof(ExternalLoginCallback)));
                Microsoft.AspNetCore.Authentication.AuthenticationProperties authenticationProperties = result.Value;
                return new ChallengeResult(externalProviderName,authenticationProperties);
            }
        }

        [HttpGet("external-register-callback-url")]
        public async Task<ActionResult> ExternalRegisterCallback()
        {
            //this command is mock up for it to pass the validator
            var result = await _sender.Send(new RegisterCustomerCommand("fake@gmail.com","123",FullName.Create("abc","abc"),isExternalRegister: true));
            if (result.IsSuccess is false)
                return MatchError(result.Errors, ModelState);
            return Ok(result.Value);
        }
        [HttpGet("external-Login-callback-url")]
        public async Task<ActionResult> ExternalLoginCallback()
        {
            //this command is mock up for it to pass the validator
            var result = await _sender.Send(new LoginCustomerQuery("123132","123123", true));
            if (result.IsSuccess is false)
                return MatchError(result.Errors, ModelState);
            return Ok(result.Value);
        }
    }
}

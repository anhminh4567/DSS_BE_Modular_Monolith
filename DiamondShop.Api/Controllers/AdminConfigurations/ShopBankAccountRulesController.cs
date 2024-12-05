using DiamondShop.Application.Usecases.AdminConfigurations.Promotions.Commands;
using DiamondShop.Application.Usecases.AdminConfigurations.Promotions.Queries;
using DiamondShop.Application.Usecases.AdminConfigurations.Promotions;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using DiamondShop.Application.Usecases.AdminConfigurations.BankAccounts.Queries;
using DiamondShop.Application.Usecases.AdminConfigurations.BankAccounts.Commands;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Application.Dtos.Responses;

namespace DiamondShop.Api.Controllers.AdminConfigurations
{
    [Route("api/Configuration/ShopBankAccountRule")]
    [Tags("Configuration")]
    [ApiController]
    public class ShopBankAccountRulesController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public ShopBankAccountRulesController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetShopBankRule()
        {
            var promotionRule = await _sender.Send(new GetShopBankAccountRuleQuery());
            var mappedRule = _mapper.Map<ShopBankAccountRulesDto>(promotionRule.Value);
            return Ok(mappedRule);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateShopBankQr([FromForm] UpdateBankAccountQrCommand request)
        {
            var updateResult = await _sender.Send(request);
            if (updateResult.IsFailed)
            {
                return MatchError(updateResult.Errors, ModelState);
            }
            return Ok(updateResult.Value);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateShopBankRule([FromBody] ShopBankAccountRulesRequestDto request)
        {
            var updateResult = await _sender.Send(new UpdateShopBankAccountRuleCommand(request));
            if (updateResult.IsFailed)
            {
                return MatchError(updateResult.Errors, ModelState);
            }
            return Ok(updateResult.Value);
        }
        public record ShopBankAccountRulesDto
        {
            public string AccountNumber { get; set; } 
            public string AccountName { get; set; } 
            public string BankBin { get; set; } 
            public string BankName { get; set; } 
            public MediaDto? BankQr { get; set; }
        }
    }
}

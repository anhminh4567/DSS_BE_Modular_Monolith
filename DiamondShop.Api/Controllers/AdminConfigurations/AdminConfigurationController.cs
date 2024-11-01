using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DiamondShop.Api.Controllers.AdminConfigurations
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminConfigurationController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;

        public AdminConfigurationController(ISender sender, IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService)
        {
            _sender = sender;
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
        }
        [HttpGet("All")]
        public async Task<ActionResult> GetAllSetting()
        {
            return Ok(_optionsMonitor.CurrentValue);
        }
    }
}

using DiamondShop.Application.Services.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace DiamondShop.Api.Controllers
{
    [Route("api/Enums")]
    [ApiController]
    [Tags("Enums")]
    public class MetadataController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly IApplicationSettingService _applicationSettingService;

        public MetadataController(IMapper mapper, ISender sender, IApplicationSettingService applicationSettingService)
        {
            _mapper = mapper;
            _sender = sender;
            _applicationSettingService = applicationSettingService;
        }

        [HttpGet]
        public async Task<ActionResult> PrintAllEnums()
        {
            var response = new Dictionary<string, Dictionary<string, int>>();

            // List of assemblies to scan
            var assemblies = new List<Assembly>
            {
                Assembly.Load("DiamondShop.Application"),
                Assembly.Load("DiamondShop.Domain")
            };

            foreach (var assembly in assemblies)
            {
                var enumTypes = assembly.GetTypes().Where(t => t.IsEnum);

                foreach (var enumType in enumTypes)
                {
                    var enumDict = Enum.GetValues(enumType)
                                       .Cast<Enum>()
                                       .ToDictionary(e => e.ToString(), e => Convert.ToInt32(e));
                    response.Add(enumType.Name, enumDict);
                }
            }
            return Ok(response);
        }
        [HttpGet("Timeline")]
        public async Task<ActionResult> PrintAllTimeline()
        {
            var response = new Dictionary<string, List<string>>();
            response.Add("Pending", new List<string> { "process", });
            response.Add("Processing", new List<string> { "process", });
            response.Add("Prepared", new List<string> { "process", });
            response.Add("Delivering", new List<string> { "process", });
            response.Add("Success", new List<string> { "process", });

            response.Add("Cancelled", new List<string> { "error" });
            response.Add("Rejected", new List<string> { "error" });
            response.Add("Delivery_Failed", new List<string> { "error" });
            response.Add("Refused", new List<string> { "error" });


            // List of assemblies to scan

            return Ok(response);
        }
        [HttpGet("Settings")]
        public async Task<ActionResult> PrintAllSettings()
        {
            return Ok();
        }
    }
}

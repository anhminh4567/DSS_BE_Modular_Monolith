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

        public MetadataController(IMapper mapper, ISender sender)
        {
            _mapper = mapper;
            _sender = sender;
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
    }
}

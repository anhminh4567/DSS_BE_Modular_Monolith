using DiamondShop.Application.Usecases.Addresses.Queries.GetAllDistricts;
using DiamondShop.Application.Usecases.Addresses.Queries.GetAllProvince;
using DiamondShop.Application.Usecases.Addresses.Queries.GetAllWards;
using DiamondShop.Domain.Common.Addresses;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Addresses
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public LocationController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("Province")]
        [Produces(typeof(List<Province>))]
        public async Task<ActionResult> GetAllProvinceSupported()
        {
            var result = await _sender.Send(new GetAllProvinceQuery());
            return Ok(result);
        }
        [HttpGet("District/{provinceId}")]
        [Produces(typeof(List<Province>))]
        public async Task<ActionResult> GetAllDistrictSupported(string provinceId)
        {
            var result = await _sender.Send(new GetAllDistrictQuery(provinceId));
            return Ok(result);
        }
        [HttpGet("Ward/{districtId}")]
        [Produces(typeof(List<Province>))]
        public async Task<ActionResult> GetAllWardSupported(string districtId)
        {
            var result = await _sender.Send(new GetAllWardQuery(districtId));
            return Ok(result);
        }
    }
}

using FluentEmail.Core.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Carts
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public CartController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

    }
}

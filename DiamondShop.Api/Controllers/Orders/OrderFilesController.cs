using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Dtos.Responses;
using DiamondShop.Application.Usecases.Orders.Files.Commands;
using DiamondShop.Application.Usecases.Orders.Files.Queries;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.DevTools;

namespace DiamondShop.Api.Controllers.Orders
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderFilesController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public OrderFilesController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult> CreateOrGetInvoice(string? orderId)
        {
            var command = new GetOrCreateOrderInvoiceCommand(orderId);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpGet("{orderId}/Files")]
        [Produces(typeof(OrderGalleryTemplate))]
        public async Task<ActionResult> GetOrderFiles([FromRoute] string orderId)
        {
            var command = new GetAllOrderFilesQuery(orderId);
            var result = await _sender.Send(command);
            if(result.IsSuccess)
            {
                var mappedResult = _mapper.Map<OrderGalleryTemplateDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}

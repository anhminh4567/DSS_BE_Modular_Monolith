﻿using DiamondShop.Application.Dtos.Responses.Deliveries;
using DiamondShop.Application.Usecases.DeliveryFees.Commands.CalculateFee;
using DiamondShop.Application.Usecases.DeliveryFees.Commands.CreateMany;
using DiamondShop.Application.Usecases.DeliveryFees.Commands.DeleteMany;
using DiamondShop.Application.Usecases.DeliveryFees.Commands.EnableDeliveryLocation;
using DiamondShop.Application.Usecases.DeliveryFees.Commands.Update;
using DiamondShop.Application.Usecases.DeliveryFees.Queries.GetAll;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Deliveries
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryFeeController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public DeliveryFeeController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpGet("All")]
        [Produces(typeof(List<DeliveryFeeDto>))]
        public async Task<ActionResult> GetAll([FromQuery]GetAllDeliveryFeeQuery getAllDeliveryFeeQuery)
        {
            var result = await _mediator.Send(getAllDeliveryFeeQuery);
            var mappedResult = _mapper.Map<List<DeliveryFeeDto>>(result);
            return Ok(mappedResult);
        }
        [HttpPost("Calculate")]
        [Produces(typeof(CalculateFeeRepsonseDto))]
        public async Task<ActionResult> CalculateFee([FromForm]CalculateFeeCommand calculateFeeCommand)
        {
            var result = await _mediator.Send(calculateFeeCommand with { isLocationCalculation = true});
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<CalculateFeeRepsonseDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("Calculate/Location")]
        [Produces(typeof(CalculateFeeRepsonseDto))]
        public async Task<ActionResult> CalculateFeeFromLocation([FromForm] CalculateFeeCommand calculateFeeCommand)
        {
            var result = await _mediator.Send(calculateFeeCommand with { isLocationCalculation = false });
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<CalculateFeeRepsonseDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost]
        [Produces(typeof(List<DeliveryFeeDto>))]
        public async Task<ActionResult> CreateMany([FromBody]CreateManyDeliveryFeeCommand createManyDeliveryFeeCommand)
        {
            var result = await _mediator.Send(createManyDeliveryFeeCommand);
            if(result.IsSuccess)
            {
                var mappedResult = _mapper.Map<List<DeliveryFeeDto>>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors,ModelState);
        }
        [HttpPut]
        [Produces(typeof(List<DeliveryFeeDto>))]
        public async Task<ActionResult> Update([FromBody] UpdateDeliveryFeesCommand updateDeliveryFeesCommand)
        {
            var result = await _mediator.Send(updateDeliveryFeesCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<List<DeliveryFeeDto>>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("SetActive")]
        public async Task<ActionResult> UpdateStatus([FromBody] EnableDeliveryLocationCommand setCommand)
        {
            var result = await _mediator.Send(setCommand);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteMany([FromBody] DeleteManyDeliveryFeesCommand deleteManyDeliveryFeesCommand)
        {
            var result = await _mediator.Send(deleteManyDeliveryFeesCommand);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return MatchError(result.Errors, ModelState);
        }
        
    }
}

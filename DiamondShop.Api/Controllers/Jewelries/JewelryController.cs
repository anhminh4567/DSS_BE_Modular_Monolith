﻿using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Application.Usecases.Jewelries.Commands.Create;
using DiamondShop.Application.Usecases.Jewelries.Commands.Delete;
using DiamondShop.Application.Usecases.Jewelries.Commands.LockForUser;
using DiamondShop.Application.Usecases.Jewelries.Commands.UpdateStatus;
using DiamondShop.Application.Usecases.Jewelries.Queries.GetAll;
using DiamondShop.Application.Usecases.Jewelries.Queries.GetAvailable;
using DiamondShop.Application.Usecases.Jewelries.Queries.GetDetail;
using DiamondShop.Application.Usecases.Jewelries.Queries.GetJewelryDiamond;
using DiamondShop.Domain.Models.RoleAggregate;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Jewelries
{
    [Route("api/Jewelry")]
    [ApiController]
    public class JewelryController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public JewelryController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpGet("Staff/All")]
        [Authorize(Roles = AccountRole.StaffId)]
        [Produces(type: typeof(PagingResponseDto<JewelryDto>))]
        public async Task<ActionResult> GetAll([FromQuery] GetAllJewelryQuery getAllJewelryQuery)
        {
            var result = await _sender.Send(getAllJewelryQuery);
            var mappedResult = _mapper.Map<PagingResponseDto<JewelryDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("Staff/Detail")]
        [Authorize(Roles = AccountRole.StaffId)]
        [Produces(type: typeof(JewelryDto))]
        public async Task<ActionResult> GetDetail([FromQuery] GetJewelryDetailQuery getJewelryDetailQuery)
        {
            var result = await _sender.Send(getJewelryDetailQuery);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<JewelryDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }

        [HttpGet("Selling")]
        [Produces(type: typeof(PagingResponseDto<JewelryDto>))]
        public async Task<ActionResult> GetSelling([FromQuery] GetJewelryDiamondQuery getJewelryPagingQuery)
        {
            var result = await _sender.Send(getJewelryPagingQuery);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<PagingResponseDto<JewelryDiamondDto>>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpGet("Available")]
        public async Task<ActionResult> GetAll([FromQuery] GetAvailableJewelryQuery getAvailableJewelryQuery)
        {
            var result = await _sender.Send(getAvailableJewelryQuery);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<string>(result.Value);
                return Ok(mappedResult);
            }
            else
                return MatchError(result.Errors, ModelState);
        }

        [HttpGet("Detail/{jewelryId}")]
        [Produces(type: typeof(JewelryDto))]
        public async Task<ActionResult> GetDetail([FromRoute] string jewelryId)
        {
            GetJewelryDetailQuery getJewelryDetailQuery = new(jewelryId);
            var result = await _sender.Send(getJewelryDetailQuery);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<JewelryDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody] CreateJewelryCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<JewelryDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("ChangeStatus")]
        public async Task<ActionResult> ChangeStatus([FromQuery] UpdateJewelryStatusCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<JewelryDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("Lock")]
        public async Task<ActionResult> Lock([FromBody] LockJewelryForUserCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<JewelryDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult> DeleteJewelry([FromQuery] DeleteJewelryCommand deleteJewelryCommand)
        {
            var result = await _sender.Send(deleteJewelryCommand);
            if (result.IsSuccess)
            {
                return Ok("Xóa trang sức thành công");
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}

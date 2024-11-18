using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Requests.Blogs;
using DiamondShop.Application.Dtos.Responses.Blogs;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Blogs.Commands.Create;
using DiamondShop.Application.Usecases.Blogs.Commands.Remove;
using DiamondShop.Application.Usecases.Blogs.Commands.Update;
using DiamondShop.Application.Usecases.Blogs.Queries.GetAll;
using DiamondShop.Application.Usecases.Blogs.Queries.GetDetail;
using DiamondShop.Domain.Models.RoleAggregate;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Blogs
{
    [Route("api/Blog")]
    [ApiController]
    public class BlogController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public BlogController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        #region Staff,Manager
        [HttpGet("Staff/All")]
        public async Task<ActionResult> GetAllStaff([FromQuery] GetAllBlogQuery getAllBlogQuery)
        {
            var result = await _sender.Send(getAllBlogQuery);
            var mappedResult = _mapper.Map<PagingResponseDto<BlogDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("Staff/Detail")]
        public async Task<ActionResult> GetDetailStaff([FromQuery] GetDetailBlogQuery getDetailBlogQuery)
        {
            var result = await _sender.Send(getDetailBlogQuery);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<BlogDto>(result.Value);
                return Ok(mappedResult);
            }
            else
                return MatchError(result.Errors, ModelState);
        }
        [HttpPost("Create")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> CreateJewelryReview([FromForm] CreateBlogRequestDto createBlogRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new CreateBlogCommand(userId.Value, createBlogRequestDto));
                if (result.IsSuccess)
                {
                    var mappedResult = _mapper.Map<BlogDto>(result.Value);
                    return Ok(mappedResult);
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        [HttpPost("Update")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> UpdateJewelryReview([FromForm] UpdateBlogRequestDto updateBlogRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new UpdateBlogCommand(userId.Value, updateBlogRequestDto));
                if (result.IsSuccess)
                {
                    var mappedResult = _mapper.Map<BlogDto>(result.Value);
                    return Ok(mappedResult);
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        [HttpPost("Remove")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> CreateJewelryReview([FromQuery] string BlogId)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new RemoveBlogCommand(BlogId, userId.Value));
                if (result.IsSuccess)
                    return Ok();
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        #endregion
        #region Customer
        [HttpGet("All")]
        public async Task<ActionResult> GetAllCustomer([FromQuery] GetAllBlogQuery getAllBlogQuery)
        {
            var result = await _sender.Send(getAllBlogQuery);
            var mappedResult = _mapper.Map<PagingResponseDto<BlogDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("Detail")]
        public async Task<ActionResult> GetDetailCustomer([FromQuery] GetDetailBlogQuery getDetailBlogQuery)
        {
            var result = await _sender.Send(getDetailBlogQuery);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<BlogDto>(result.Value);
                return Ok(mappedResult);
            }
            else
                return MatchError(result.Errors, ModelState);
        }
        #endregion
    }
}

using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.Blogs;
using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Blogs.Commands.Create;
using DiamondShop.Application.Usecases.Blogs.Queries.GetAll;
using DiamondShop.Application.Usecases.JewelryReviews.Commands.Create;
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
        #endregion
        [HttpGet("All")]
        public async Task<ActionResult> GetAll(GetAllBlogQuery getAllBlogQuery)
        {
            var result = await _sender.Send(getAllBlogQuery);
            var mappedResult = _mapper.Map<PagingResponseDto<BlogDto>>(result);
            return Ok(mappedResult);
        }
    }
}

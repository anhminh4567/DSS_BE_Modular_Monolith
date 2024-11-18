using Microsoft.AspNetCore.Http;

namespace DiamondShop.Application.Dtos.Requests.Blogs
{
    public record CreateBlogRequestDto(string Title, List<string> BlogTags, IFormFile? Thumbnail, string Content);
}

using Microsoft.AspNetCore.Http;

namespace DiamondShop.Application.Dtos.Requests.Blogs
{
    public record UpdateBlogRequestDto(string BlogId, string Title, List<string> BlogTags, IFormFile Thumbnail, IFormFile[] Contents);

}

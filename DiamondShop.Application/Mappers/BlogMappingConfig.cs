using DiamondShop.Application.Dtos.Responses.Blogs;
using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Models.Blogs.Entities;
using Mapster;

namespace DiamondShop.Application.Mappers
{
    public class BlogMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Blog, BlogDto>();

            config.NewConfig<BlogTag, string>()
                .Map(dest => dest, src => src.Value);
        }
    }
}

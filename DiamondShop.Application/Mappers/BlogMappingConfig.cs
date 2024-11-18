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
            config.NewConfig<Blog, BlogDto>()
                .Map(dest => dest.Tags, src => src.Tags.Select(p => p.Value).ToList());
        }
    }
}

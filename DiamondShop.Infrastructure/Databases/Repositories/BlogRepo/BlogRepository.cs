using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Models.Blogs.ValueObjects;
using DiamondShop.Domain.Repositories.BlogRepo;
using Microsoft.EntityFrameworkCore;

namespace DiamondShop.Infrastructure.Databases.Repositories.BlogRepo
{
    internal class BlogRepository : BaseRepository<Blog>, IBlogRepository
    {
        public BlogRepository(DiamondShopDbContext context) : base(context) { }
        public override async Task<Blog?> GetById(params object[] ids)
        {
            var id = (BlogId)ids[0];
            return await _set.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
